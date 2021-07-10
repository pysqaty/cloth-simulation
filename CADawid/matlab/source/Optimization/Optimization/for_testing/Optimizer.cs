/*
* MATLAB Compiler: 8.1 (R2020b)
* Date: Sun May 23 12:29:49 2021
* Arguments:
* "-B""macro_default""-W""dotnet:Optimization,Optimizer,4.0,private,version=1.0""-T""link:
* lib""-d""C:\Users\dawid\source\repos\masterthesis\CADawid\matlab\source\Optimization\Opt
* imization\for_testing""-v""class{Optimizer:C:\Users\dawid\source\repos\masterthesis\CADa
* wid\matlab\source\ic_barrier.m,C:\Users\dawid\source\repos\masterthesis\CADawid\matlab\s
* ource\initConstraints.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace Optimization
{

  /// <summary>
  /// The Optimizer class provides a CLS compliant, MWArray interface to the MATLAB
  /// functions contained in the files:
  /// <newpara></newpara>
  /// C:\Users\dawid\source\repos\masterthesis\CADawid\matlab\source\ic_barrier.m
  /// <newpara></newpara>
  /// C:\Users\dawid\source\repos\masterthesis\CADawid\matlab\source\initConstraints.m
  /// </summary>
  /// <remarks>
  /// @Version 1.0
  /// </remarks>
  public class Optimizer : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static Optimizer()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          Assembly assembly= Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

		  int lastDelimiter = ctfFilePath.LastIndexOf(@"/");

	      if (lastDelimiter == -1)
		  {
		    lastDelimiter = ctfFilePath.LastIndexOf(@"\");
		  }

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "Optimization.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the Optimizer class.
    /// </summary>
    public Optimizer()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~Optimizer()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier()
    {
      return mcr.EvaluateFunction("ic_barrier", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses)
    {
      return mcr.EvaluateFunction("ic_barrier", masses);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam);
    }


    /// <summary>
    /// Provides a single output, 8-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam, 
                        MWArray initTrustRegionRadiusParam)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam);
    }


    /// <summary>
    /// Provides a single output, 9-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam, 
                        MWArray initTrustRegionRadiusParam, MWArray stepToleranceParam)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam);
    }


    /// <summary>
    /// Provides a single output, 10-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam, 
                        MWArray initTrustRegionRadiusParam, MWArray stepToleranceParam, 
                        MWArray maxFunctionEvaluationsParam)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam);
    }


    /// <summary>
    /// Provides a single output, 11-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam, 
                        MWArray initTrustRegionRadiusParam, MWArray stepToleranceParam, 
                        MWArray maxFunctionEvaluationsParam, MWArray 
                        constraintToleranceParam)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam);
    }


    /// <summary>
    /// Provides a single output, 12-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <param name="optimalityToleranceParam">Input argument #12</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam, 
                        MWArray initTrustRegionRadiusParam, MWArray stepToleranceParam, 
                        MWArray maxFunctionEvaluationsParam, MWArray 
                        constraintToleranceParam, MWArray optimalityToleranceParam)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam, optimalityToleranceParam);
    }


    /// <summary>
    /// Provides a single output, 13-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <param name="optimalityToleranceParam">Input argument #12</param>
    /// <param name="maxIterationsParam">Input argument #13</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam, 
                        MWArray initTrustRegionRadiusParam, MWArray stepToleranceParam, 
                        MWArray maxFunctionEvaluationsParam, MWArray 
                        constraintToleranceParam, MWArray optimalityToleranceParam, 
                        MWArray maxIterationsParam)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam, optimalityToleranceParam, maxIterationsParam);
    }


    /// <summary>
    /// Provides a single output, 14-input MWArrayinterface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <param name="optimalityToleranceParam">Input argument #12</param>
    /// <param name="maxIterationsParam">Input argument #13</param>
    /// <param name="clothId">Input argument #14</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray ic_barrier(MWArray masses, MWArray systemState, MWArray constraints, 
                        MWArray k, MWArray g, MWArray dt, MWArray initBarrierParam, 
                        MWArray initTrustRegionRadiusParam, MWArray stepToleranceParam, 
                        MWArray maxFunctionEvaluationsParam, MWArray 
                        constraintToleranceParam, MWArray optimalityToleranceParam, 
                        MWArray maxIterationsParam, MWArray clothId)
    {
      return mcr.EvaluateFunction("ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam, optimalityToleranceParam, maxIterationsParam, clothId);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam);
    }


    /// <summary>
    /// Provides the standard 8-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam, MWArray initTrustRegionRadiusParam)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam);
    }


    /// <summary>
    /// Provides the standard 9-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam, MWArray initTrustRegionRadiusParam, MWArray 
                          stepToleranceParam)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam);
    }


    /// <summary>
    /// Provides the standard 10-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam, MWArray initTrustRegionRadiusParam, MWArray 
                          stepToleranceParam, MWArray maxFunctionEvaluationsParam)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam);
    }


    /// <summary>
    /// Provides the standard 11-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam, MWArray initTrustRegionRadiusParam, MWArray 
                          stepToleranceParam, MWArray maxFunctionEvaluationsParam, 
                          MWArray constraintToleranceParam)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam);
    }


    /// <summary>
    /// Provides the standard 12-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <param name="optimalityToleranceParam">Input argument #12</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam, MWArray initTrustRegionRadiusParam, MWArray 
                          stepToleranceParam, MWArray maxFunctionEvaluationsParam, 
                          MWArray constraintToleranceParam, MWArray 
                          optimalityToleranceParam)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam, optimalityToleranceParam);
    }


    /// <summary>
    /// Provides the standard 13-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <param name="optimalityToleranceParam">Input argument #12</param>
    /// <param name="maxIterationsParam">Input argument #13</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam, MWArray initTrustRegionRadiusParam, MWArray 
                          stepToleranceParam, MWArray maxFunctionEvaluationsParam, 
                          MWArray constraintToleranceParam, MWArray 
                          optimalityToleranceParam, MWArray maxIterationsParam)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam, optimalityToleranceParam, maxIterationsParam);
    }


    /// <summary>
    /// Provides the standard 14-input MWArray interface to the ic_barrier MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="masses">Input argument #1</param>
    /// <param name="systemState">Input argument #2</param>
    /// <param name="constraints">Input argument #3</param>
    /// <param name="k">Input argument #4</param>
    /// <param name="g">Input argument #5</param>
    /// <param name="dt">Input argument #6</param>
    /// <param name="initBarrierParam">Input argument #7</param>
    /// <param name="initTrustRegionRadiusParam">Input argument #8</param>
    /// <param name="stepToleranceParam">Input argument #9</param>
    /// <param name="maxFunctionEvaluationsParam">Input argument #10</param>
    /// <param name="constraintToleranceParam">Input argument #11</param>
    /// <param name="optimalityToleranceParam">Input argument #12</param>
    /// <param name="maxIterationsParam">Input argument #13</param>
    /// <param name="clothId">Input argument #14</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] ic_barrier(int numArgsOut, MWArray masses, MWArray systemState, 
                          MWArray constraints, MWArray k, MWArray g, MWArray dt, MWArray 
                          initBarrierParam, MWArray initTrustRegionRadiusParam, MWArray 
                          stepToleranceParam, MWArray maxFunctionEvaluationsParam, 
                          MWArray constraintToleranceParam, MWArray 
                          optimalityToleranceParam, MWArray maxIterationsParam, MWArray 
                          clothId)
    {
      return mcr.EvaluateFunction(numArgsOut, "ic_barrier", masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam, stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam, optimalityToleranceParam, maxIterationsParam, clothId);
    }


    /// <summary>
    /// Provides an interface for the ic_barrier function in which the input and output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void ic_barrier(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("ic_barrier", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a void output, 0-input MWArrayinterface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    ///
    public void initConstraints()
    {
      mcr.EvaluateFunction(0, "initConstraints", new MWArray[]{});
    }


    /// <summary>
    /// Provides a void output, 1-input MWArrayinterface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="systemState">Input argument #1</param>
    ///
    public void initConstraints(MWArray systemState)
    {
      mcr.EvaluateFunction(0, "initConstraints", systemState);
    }


    /// <summary>
    /// Provides a void output, 2-input MWArrayinterface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="systemState">Input argument #1</param>
    /// <param name="alpha">Input argument #2</param>
    ///
    public void initConstraints(MWArray systemState, MWArray alpha)
    {
      mcr.EvaluateFunction(0, "initConstraints", systemState, alpha);
    }


    /// <summary>
    /// Provides a void output, 3-input MWArrayinterface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="systemState">Input argument #1</param>
    /// <param name="alpha">Input argument #2</param>
    /// <param name="clothId">Input argument #3</param>
    ///
    public void initConstraints(MWArray systemState, MWArray alpha, MWArray clothId)
    {
      mcr.EvaluateFunction(0, "initConstraints", systemState, alpha, clothId);
    }


    /// <summary>
    /// Provides a void output, 4-input MWArrayinterface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="systemState">Input argument #1</param>
    /// <param name="alpha">Input argument #2</param>
    /// <param name="clothId">Input argument #3</param>
    /// <param name="colliders">Input argument #4</param>
    ///
    public void initConstraints(MWArray systemState, MWArray alpha, MWArray clothId, 
                          MWArray colliders)
    {
      mcr.EvaluateFunction(0, "initConstraints", systemState, alpha, clothId, colliders);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] initConstraints(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "initConstraints", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="systemState">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] initConstraints(int numArgsOut, MWArray systemState)
    {
      return mcr.EvaluateFunction(numArgsOut, "initConstraints", systemState);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="systemState">Input argument #1</param>
    /// <param name="alpha">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] initConstraints(int numArgsOut, MWArray systemState, MWArray alpha)
    {
      return mcr.EvaluateFunction(numArgsOut, "initConstraints", systemState, alpha);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="systemState">Input argument #1</param>
    /// <param name="alpha">Input argument #2</param>
    /// <param name="clothId">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] initConstraints(int numArgsOut, MWArray systemState, MWArray alpha, 
                               MWArray clothId)
    {
      return mcr.EvaluateFunction(numArgsOut, "initConstraints", systemState, alpha, clothId);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the initConstraints MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// init gradients
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="systemState">Input argument #1</param>
    /// <param name="alpha">Input argument #2</param>
    /// <param name="clothId">Input argument #3</param>
    /// <param name="colliders">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] initConstraints(int numArgsOut, MWArray systemState, MWArray alpha, 
                               MWArray clothId, MWArray colliders)
    {
      return mcr.EvaluateFunction(numArgsOut, "initConstraints", systemState, alpha, clothId, colliders);
    }



    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}
