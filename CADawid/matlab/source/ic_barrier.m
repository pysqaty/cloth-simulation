function [xnext, cforce] = ic_barrier(masses, systemState, constraints, k, g, dt, initBarrierParam, initTrustRegionRadiusParam,...
    stepToleranceParam, maxFunctionEvaluationsParam, constraintToleranceParam, optimalityToleranceParam, maxIterationsParam, clothId)

global cId;

cId = clothId;

xn = zeros(systemState.PointStates.Length * 3, 1);
vn = zeros(systemState.PointStates.Length * 3, 1);
ii = 1;
for i=1:systemState.PointStates.Length
    xn(ii) = systemState.PointStates(i).X.X;
    xn(ii + 1) = systemState.PointStates(i).X.Y;
    xn(ii + 2) = systemState.PointStates(i).X.Z;
    vn(ii) = systemState.PointStates(i).V.X;
    vn(ii + 1) = systemState.PointStates(i).V.Y;
    vn(ii + 2) = systemState.PointStates(i).V.Z;
    ii = ii + 3;
end

M = diag(repelem(masses,3));
G = repmat([0;1;0], systemState.PointStates.Length, 1)*g;
Eh = @(x)E(x, xn, vn, M, G, dt, k);
A = [];
b = [];
Aeq = [];
beq = [];
if constraints.Count ~= 0
    Aeq = zeros(constraints.Count, length(xn));
    beq = zeros(constraints.Count, 1);
    for i=1:1:constraints.Count
        Aeq(i, constraints.Item(i - 1).Index + 1) = 1;
        beq(i) = constraints.Item(i - 1).Value;
    end
end
lb = [];
ub = [];

nonlcon = @inequality_constraints;

options = optimoptions('fmincon', 'SpecifyObjectiveGradient', true, 'SpecifyConstraintGradient', true, 'StepTolerance', stepToleranceParam,...
    'MaxFunctionEvaluations', maxFunctionEvaluationsParam, 'InitBarrierParam', initBarrierParam,...
    'InitTrustRegionRadius', initTrustRegionRadiusParam, 'ConstraintTolerance', constraintToleranceParam, 'OptimalityTolerance', optimalityToleranceParam,...
    'MaxIterations', maxIterationsParam, 'Algorithm', 'interior-point', 'SubproblemAlgorithm', 'factorization'); 
tic
[xnext,~,~,~,lambda] = fmincon(Eh,xn,A,b,Aeq,beq,lb,ub,nonlcon,options);
toc
global DC;
cforce = DC*lambda.ineqnonlin;
end

function [f, gradF] = E(x, xn, vn, M, G, dt, k)
ksi = @(x)((k/(2*dt))*(x-xn)'*M*(x-xn));
f = (0.5*(x-xn)'*M*(x-xn)-dt*(x-xn)'*M*vn+dt*dt*ksi(x)+dt*dt*(x-xn)'*M*G);
if nargout > 1
    gradF = (1 + dt*k)*M*(x-xn) - dt*M*(vn + dt*G);
end
end

function [c,ceq,DC,DCeq] = inequality_constraints(x)
global C gradC cId;
global DC;
c = zeros(length(C{cId}), 1);
ceq = [];
for k = 1:length(C{cId})
   c(k) = C{cId}{k}(x);
end
if nargout > 2
    DC = zeros(length(x), length(C{cId}));
    for i=1:length(x)
        for j=1:length(C{cId})
            DC(i,j) = gradC{cId}{i,j}(x);
        end
    end
    DCeq = [];
end
end