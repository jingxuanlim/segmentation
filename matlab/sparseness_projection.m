function S = sparseness_projection(S, s)

d = numel(S);
S = reshape(S, [d, 1]);

S = max(S, 0);
if s <= 0;
    return;
end

L1 = sum(S);                            % fixed l1-norm
L2 = L1 / (sqrt(d) * (1 - s) + s);      % desired l2-norm

if sqrt(sum(S.^2)) > L2
    return;
end

% Initialize components with negative values
Z = false(d, 1);

negatives = true;
while  negatives
    % Fix components with negative values at 0
    Z    = Z | (S<0);
    S(Z) = 0;
    
    % Project to the sum-constraint hyperplane
    S    = S + (L1 - sum(S)) / (d - nnz(Z));
    S(Z) = 0;
    
    % Get midpoints of hyperplane, M
    M    = repmat(L1 / (d - nnz(Z)), [d, 1]);
    M(Z) = 0;
    P    = S - M;
    
    % Solve L2 = l2[M + Alph.*(S-M)] = l2[W.*Alph + M], for Alph
    A    =   sum(P .* P);
    B    = 2*sum(P .* M);
    C    =   sum(M .* M) - L2.^2;
    
    Alph = (-B + real(sqrt(B.^2 - 4 * A .* C)))./(2 * A);
    
    % Project within the sum-constraint hyperplane to match L2
    S    = M + Alph * P;
    
    % Check for negative values in solution;
    negatives = any(S < 0);
end
