function [] = initConstraints(systemState, alpha, clothId, colliders)
global C gradC;

oneC = [];
oneGradC = [];

k = 1;
alreadyConn = [-1 -1];
for i=1:systemState.Connections.Length
    for j=1:systemState.Connections(i).Connections.Length
        connection = systemState.Connections(i).Connections(j);
        if any(ismember(alreadyConn, [connection.to connection.from], 'rows'))
            continue;
        end
        if any(ismember(alreadyConn, [connection.from connection.to], 'rows'))
            continue;
        end
        alreadyConn = [alreadyConn; [connection.to connection.from]; [connection.from connection.to]];
        l0 = connection.l0;
        constr = @(x)(double(norm(x(3 * (connection.to + 1) - 2 : 3 * (connection.to + 1)) - x(3 * (connection.from + 1) - 2 : 3 * (connection.from + 1))) / ((1 + alpha)*l0) - 1));
        oneC{k} = constr;
        for gradi=1:systemState.PointStates.Length*3
            oneGradC{gradi, k} = @(x)(0);
        end
        oneGradC{3 * (connection.to + 1) - 2, k} = @(x)double((x(3 * (connection.to + 1) - 2)-x(3 * (connection.from + 1) - 2)) / ((1 + alpha)*l0*norm(x(3 * (connection.to + 1) - 2 : 3 * (connection.to + 1)) - x(3 * (connection.from + 1) - 2 : 3 * (connection.from + 1)))));
        oneGradC{3 * (connection.to + 1) - 1, k} = @(x)double((x(3 * (connection.to + 1) - 1)-x(3 * (connection.from + 1) - 1)) / ((1 + alpha)*l0*norm(x(3 * (connection.to + 1) - 2 : 3 * (connection.to + 1)) - x(3 * (connection.from + 1) - 2 : 3 * (connection.from + 1)))));
        oneGradC{3 * (connection.to + 1), k} = @(x)double((x(3 * (connection.to + 1))-x(3 * (connection.from + 1))) / ((1 + alpha)*l0*norm(x(3 * (connection.to + 1) - 2 : 3 * (connection.to + 1)) - x(3 * (connection.from + 1) - 2 : 3 * (connection.from + 1)))));
        oneGradC{3 * (connection.from + 1) - 2, k} = @(x)double(-(x(3 * (connection.to + 1) - 2)-x(3 * (connection.from + 1) - 2)) / ((1 + alpha)*l0*norm(x(3 * (connection.to + 1) - 2 : 3 * (connection.to + 1)) - x(3 * (connection.from + 1) - 2 : 3 * (connection.from + 1)))));
        oneGradC{3 * (connection.from + 1) - 1, k} = @(x)double(-(x(3 * (connection.to + 1) - 1)-x(3 * (connection.from + 1) - 1)) / ((1 + alpha)*l0*norm(x(3 * (connection.to + 1) - 2 : 3 * (connection.to + 1)) - x(3 * (connection.from + 1) - 2 : 3 * (connection.from + 1)))));
        oneGradC{3 * (connection.from + 1), k} = @(x)double(-(x(3 * (connection.to + 1))-x(3 * (connection.from + 1))) / ((1 + alpha)*l0*norm(x(3 * (connection.to + 1) - 2 : 3 * (connection.to + 1)) - x(3 * (connection.from + 1) - 2 : 3 * (connection.from + 1)))));
        
        k = k + 1;
    end
end

for i=1:colliders.Length
    collider = colliders(i);
    for j=1:3:3*systemState.PointStates.Length
        constr = @(x)(-collider.Sdf(x(j), x(j + 1), x(j + 2)));
        oneC{k} = constr;
        
        %init gradients
        for gradi=1:systemState.PointStates.Length*3
            oneGradC{gradi, k} = @(x)(0);
        end
        oneGradC{j, k} = @(x)(-collider.dxSdf(x(j), x(j + 1), x(j + 2)));
        oneGradC{j + 1, k} = @(x)(-collider.dySdf(x(j), x(j + 1), x(j + 2)));
        oneGradC{j + 2, k} = @(x)(-collider.dzSdf(x(j), x(j + 1), x(j + 2)));
        
        k = k + 1;
    end
end

C{clothId} = oneC;
gradC{clothId} = oneGradC;

end