using System;
using CADawid.Model;
using CADawid.Model.Scene;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;

namespace CADawid.DxModule
{
    public class DxRenderer
    {
        public object renderMutex;

        float t;
        public int Height = 0;
        public int Width = 0;

        private DriverType driverType;
        public Device device;
        private RenderTargetView renderTargetView;
        private RenderTargetView renderTargetViewT2;
        public Texture2D renderedT2;

        public InputLayout inputLayout;


        public VertexShader vertexShader;
        public PixelShader pixelShader;

        public VertexShader tess_vertexShader;
        public PixelShader tess_pixelShader;
        public HullShader tess_hullShader;
        public DomainShader tess_domainShader;

        public InputLayout phongInputLayout;
        public VertexShader phongVertexShader;
        public PixelShader phongPixelShader;

        public VertexShader phongTVertexShader;
        public PixelShader phongTPixelShader;

        public SharpDX.Direct3D11.Buffer constantBuffer;
        public SharpDX.Direct3D11.Buffer viewConstantBuffer;
        private DepthStencilState depthStencilStateOn;
        private DepthStencilView depthStencilView;

        public SharpDX.Direct3D11.Buffer controlPoints;
        public ShaderResourceView srv;

        public Scene Scene { get; set; }

        public DxRenderer()
        {
            renderMutex = new object();
            Height = 0;
            Width = 0;
            device = null;
            renderTargetView = null;
            inputLayout = null;
            vertexShader = null;
            pixelShader = null;
            constantBuffer = null;
            Scene = new Scene_StairsColliding();
            InitDevice();
        }
        private void InitDevice()
        {
            DeviceCreationFlags createDeviceFlags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug;
            DriverType[] driverTypes = new DriverType[]
            {
                DriverType.Hardware,
                DriverType.Warp,
                DriverType.Reference
            };
            int numDriverTypes = driverTypes.Length;

            FeatureLevel[] featureLevels = new FeatureLevel[]
            {
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0
            };
            int numFeatureLevels = featureLevels.Length;

            for (int i = 0; i < numDriverTypes; i++)
            {
                device = new SharpDX.Direct3D11.Device(driverTypes[i],
                    createDeviceFlags, featureLevels);
                if (device != null)
                {
                    driverType = driverTypes[i];
                    break;
                }
            }

            InputElement[] layout = new InputElement[]
            {
                new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float,
                    0, 0, InputClassification.PerVertexData, 0),
            };
            InputElement[] phongLayout = new InputElement[]
            {
                new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float,
                    0, 0, InputClassification.PerVertexData, 0),
                new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32A32_Float,
                        InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
            };

            (vertexShader, pixelShader, inputLayout) = LoadShaders("../../../Shader/basicShader.fx", layout);
            (phongVertexShader, phongPixelShader, phongInputLayout) = LoadShaders("../../../Shader/phongShader.fx", phongLayout);
            (phongTVertexShader, phongTPixelShader, _) = LoadShaders("../../../Shader/phongTriangleShader.fx", phongLayout);
            (tess_vertexShader, tess_pixelShader, tess_hullShader, tess_domainShader, inputLayout) = LoadTessShaders("../../../Shader/bezierCubeShader.fx", layout);
            device.ImmediateContext.InputAssembler.InputLayout = inputLayout;
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;


            controlPoints = new SharpDX.Direct3D11.Buffer(device, new BufferDescription()
            {
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.Write,
                Usage = ResourceUsage.Dynamic,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 64 * 4 * 4 * 8,
                StructureByteStride = 4 * 4 * 4
            });


            srv = new ShaderResourceView(device, controlPoints, new ShaderResourceViewDescription()
            {
                Dimension = ShaderResourceViewDimension.Buffer,
                Format = SharpDX.DXGI.Format.R32G32B32A32_Float,
                Buffer = new ShaderResourceViewDescription.BufferResource
                {
                    ElementCount = 64,
                    ElementOffset = 0,
                    ElementWidth = 4 * 32,
                    FirstElement = 0
                }
            });

            device.ImmediateContext.VertexShader.SetShaderResource(0, srv);

            CreateConstantBuffer();
        }
        private (VertexShader vs, PixelShader ps, InputLayout inputLayout) LoadShaders(string shaderPath, InputElement[] layout)
        {
            string fullPath = System.IO.Path.GetFullPath(shaderPath);
            byte[] vSByteCode = ShaderBytecode.CompileFromFile(fullPath, "VS", "vs_5_0");
            byte[] pSByteCode = ShaderBytecode.CompileFromFile(fullPath, "PS", "ps_5_0");
            var vs= new VertexShader(device, vSByteCode);
            var ps = new PixelShader(device, pSByteCode);

            var il = new InputLayout(device, vSByteCode, layout);

            return (vs, ps, il);
        }

        private (VertexShader vs, PixelShader ps, HullShader hs, DomainShader ds, InputLayout inputLayout) LoadTessShaders(string shaderPath, InputElement[] layout)
        {
            try
            {
                string fullPath = System.IO.Path.GetFullPath(shaderPath);
                byte[] vSByteCode = ShaderBytecode.CompileFromFile(fullPath, "VS", "vs_5_0");
                byte[] pSByteCode = ShaderBytecode.CompileFromFile(fullPath, "PS", "ps_5_0");
                byte[] hSByteCode = ShaderBytecode.CompileFromFile(fullPath, "HS", "hs_5_0");
                byte[] dSByteCode = ShaderBytecode.CompileFromFile(fullPath, "DS", "ds_5_0");
                var vs = new VertexShader(device, vSByteCode);
                var ps = new PixelShader(device, pSByteCode);
                var hs = new HullShader(device, hSByteCode);
                var ds = new DomainShader(device, dSByteCode);

                var il = new InputLayout(device, vSByteCode, layout);
                return (vs, ps, hs, ds, il);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return (null, null, null, null, null);
        }

        private void SetUpViewPort()
        {
            SharpDX.Mathematics.Interop.RawViewportF vp = new SharpDX.Mathematics.Interop.RawViewportF();
            vp.Width = Width;
            vp.Height = Height;
            vp.MinDepth = 0f;
            vp.MaxDepth = 1f;
            vp.X = 0;
            vp.Y = 0;
            device.ImmediateContext.Rasterizer.SetViewport(vp);

            float size = 50;
            Scene.Camera.width = size;
            Scene.Camera.height = Height * size / Width;
            Scene.Camera.ProjectionMatrix = SharpDX.Matrix.PerspectiveFovLH((float)Math.PI / 4f, Width / (float)Height,
                0.01f, 100.0f);

        }
        private void InitRenderTarget(IntPtr resourcePointer)
        {
            lock(renderMutex)
            {
                ComObject pUnk = new ComObject(resourcePointer);
                SharpDX.DXGI.Resource pDXGIResource = pUnk.QueryInterface<SharpDX.DXGI.Resource>();
                IntPtr sharedHandle = pDXGIResource.SharedHandle;

                SharpDX.Direct3D11.Resource tempResource11 = device.OpenSharedResource<SharpDX.Direct3D11.Resource>(sharedHandle);
                Texture2D pOutputResource = tempResource11.QueryInterface<Texture2D>();

                RenderTargetViewDescription rtDesc = new RenderTargetViewDescription();
                rtDesc.Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm;
                rtDesc.Dimension = RenderTargetViewDimension.Texture2D;
                rtDesc.Texture2D = new RenderTargetViewDescription.Texture2DResource();
                rtDesc.Texture2D.MipSlice = 0;

                renderTargetView = new RenderTargetView(device, pOutputResource, rtDesc);

                Texture2DDescription outputResourceDesc = pOutputResource.Description;

                if (outputResourceDesc.Width != Width || outputResourceDesc.Height != Height)
                {
                    Width = outputResourceDesc.Width;
                    Height = outputResourceDesc.Height;
                    SetUpViewPort();
                }

                device.ImmediateContext.OutputMerger.SetRenderTargets(renderTargetView);

                var rstDesc = new RasterizerStateDescription
                {
                    CullMode = CullMode.None,
                    FillMode = FillMode.Solid
                };
                var rsFillState = new RasterizerState(device, rstDesc);
                device.ImmediateContext.Rasterizer.State = rsFillState;

                Texture2DDescription depthTexture = new Texture2DDescription();
                depthTexture.Width = outputResourceDesc.Width;
                depthTexture.Height = outputResourceDesc.Height;
                depthTexture.ArraySize = 1;
                depthTexture.Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt;
                depthTexture.SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0);
                depthTexture.Usage = ResourceUsage.Default;
                depthTexture.BindFlags = BindFlags.DepthStencil;
                depthTexture.MipLevels = 1;
                depthTexture.CpuAccessFlags = CpuAccessFlags.None;
                depthTexture.OptionFlags = ResourceOptionFlags.None;
                depthTexture.MipLevels = 0;

                Texture2D depthStencilBuffer = new Texture2D(device, depthTexture);

                DepthStencilStateDescription depthStencilDescOn = new DepthStencilStateDescription()
                {
                    IsDepthEnabled = true,
                    DepthWriteMask = DepthWriteMask.All,
                    DepthComparison = Comparison.Less,
                    IsStencilEnabled = true,
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    }
                };

                DepthStencilViewDescription depthStencilViewDesc = new DepthStencilViewDescription()
                {
                    Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource()
                    {
                        MipSlice = 0
                    }
                };

                depthStencilStateOn = new DepthStencilState(device, depthStencilDescOn);
                device.ImmediateContext.OutputMerger.SetDepthStencilState(depthStencilStateOn, 1);


                depthStencilView = new DepthStencilView(device, depthStencilBuffer, depthStencilViewDesc);


                // TEXTURE RENDERING --------------------------------------------------------------------

                Texture2DDescription textureDesc = new Texture2DDescription();
                textureDesc.Width = Width;
                textureDesc.Height = Height;
                textureDesc.MipLevels = 1;
                textureDesc.ArraySize = 1;
                textureDesc.Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm;
                textureDesc.SampleDescription.Count = 1;
                textureDesc.Usage = ResourceUsage.Default;
                textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
                textureDesc.CpuAccessFlags = 0;
                textureDesc.OptionFlags = 0;
                renderedT2 = new Texture2D(device, textureDesc);

                RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription();
                renderTargetViewDesc.Format = textureDesc.Format;
                renderTargetViewDesc.Dimension = RenderTargetViewDimension.Texture2D;
                renderTargetViewDesc.Texture2D = new RenderTargetViewDescription.Texture2DResource();
                renderTargetViewDesc.Texture2D.MipSlice = 0;

                renderTargetViewT2 = new RenderTargetView(device, renderedT2, renderTargetViewDesc);


                device.ImmediateContext.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView, renderTargetViewT2);

                pOutputResource?.Dispose();
                tempResource11?.Dispose();
                pDXGIResource?.Dispose();
            }
        }
        public bool Render(IntPtr resourcePointer, bool isNewSurface)
        {
            lock(renderMutex)
            {
                if (isNewSurface)
                {
                    renderTargetView = null;
                    device.ImmediateContext.OutputMerger.SetRenderTargets(renderTargetView: null);
                    InitRenderTarget(resourcePointer);
                }

                RawColor4 clearColor = new RawColor4(0.0f, 0.0f, 0.0f, 1.0f);
                device.ImmediateContext.ClearRenderTargetView(renderTargetView, clearColor);
                device.ImmediateContext.ClearRenderTargetView(renderTargetViewT2, clearColor);

                device.ImmediateContext.ClearDepthStencilView(depthStencilView,
                    DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

                Scene.Camera.Update();

                lock (Scene.ClothSimulation.mutex)
                {
                    foreach (IGeometryObject geometry in Scene.Geometries)
                    {
                        geometry.Render(this);
                    }
                }


                if (device.ImmediateContext != null)
                {
                    device.ImmediateContext.Flush();
                }
                return true;
            }
        }

        public SharpDX.Direct3D11.Buffer CreateVertexBuffer<T>(T[] vertices, int tSize, bool isDynamic = false) where T : struct
        {
            BufferDescription bd = new BufferDescription();
            bd.Usage = isDynamic ? ResourceUsage.Dynamic : ResourceUsage.Default;
            bd.CpuAccessFlags = isDynamic ? CpuAccessFlags.Write : CpuAccessFlags.None;
            bd.BindFlags = BindFlags.VertexBuffer;
            unsafe
            {
                bd.SizeInBytes = tSize * vertices.Length;
            }

            return SharpDX.Direct3D11.Buffer.Create<T>(device, vertices, bd);
        }
        public void SetVertexBuffer<T>(SharpDX.Direct3D11.Buffer vertexBuffer, int tSize)
        {
            int stride = 0;
            unsafe
            {
                stride = tSize;
            }
            int offset = 0;
            var vBB = new VertexBufferBinding(vertexBuffer, stride, offset);
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, vBB);
        }
        public SharpDX.Direct3D11.Buffer CreateIndexBuffer<T>(T[] indices, int tSize) where T : struct
        {
            BufferDescription bd = new BufferDescription();
            bd.Usage = ResourceUsage.Default;
            unsafe
            {
                bd.SizeInBytes = tSize * indices.Length;
            }

            bd.BindFlags = BindFlags.IndexBuffer;
            bd.CpuAccessFlags = CpuAccessFlags.None;
            return SharpDX.Direct3D11.Buffer.Create(device, indices, bd);
        }
        public void SetIndexBuffer(SharpDX.Direct3D11.Buffer indicesBuffer, SharpDX.DXGI.Format format = SharpDX.DXGI.Format.R16_UInt)
        {
            device.ImmediateContext.InputAssembler.SetIndexBuffer(indicesBuffer, format, 0);
        }
        private void CreateConstantBuffer()
        {
            BufferDescription bd = new BufferDescription();
            bd.Usage = ResourceUsage.Default;
            unsafe
            {
                bd.SizeInBytes = sizeof(DxConstantBuffer);
            }
            bd.BindFlags = BindFlags.ConstantBuffer;
            bd.CpuAccessFlags = CpuAccessFlags.None;
            constantBuffer = new SharpDX.Direct3D11.Buffer(device, bd);

            BufferDescription bd2 = new BufferDescription();
            bd2.Usage = ResourceUsage.Default;
            unsafe
            {
                bd2.SizeInBytes = sizeof(DxViewConstantBuffer);
            }
            bd2.BindFlags = BindFlags.ConstantBuffer;
            bd2.CpuAccessFlags = CpuAccessFlags.None;
            viewConstantBuffer = new SharpDX.Direct3D11.Buffer(device, bd2);
        }

    }
}
