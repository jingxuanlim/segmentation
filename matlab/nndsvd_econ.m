function [W, H, U, S, V] = nndsvd_econ(A)
%
% Inputs:
% A, nonnegative m x n matrix A
% k, rank of W and H
%
% Outpus:
% W, nonnegative m x k matrix
% H, nonnegative k x n matrix

% assert non-negativity
% assert(all(all(A >= 0)))

% size of the input matrix
[m, n] = size(A);

% 1st SVD --> partial SVD rank-k to the input matrix A.
[U, S, V] = svd(A, 'econ');

% the matrices of the factorization
k = size(S, 1);
W = zeros(m, k);
H = zeros(k, n);

% choose the first singular triplet to be nonnegative
W(:, 1) = sqrt(S(1, 1)) * abs(U(:, 1)  );
H(1, :) = sqrt(S(1, 1)) * abs(V(:, 1).');

% 2nd SVD for the other factors (see table 1 in our paper)
for i = 2:k
    uu    =   U(:, i);              vv    =   V(:, i);
    uup   =   uu .* (uu > 0);       vvp   =   vv .* (vv > 0);
    uun   = - uu .* (uu < 0);       vvn   = - vv .* (vv < 0);
    n_uup = norm(uup);              n_vvp = norm(vvp);
    n_uun = norm(uun);              n_vvn = norm(vvn);
    
    termp = n_uup * n_vvp;
    termn = n_uun * n_vvn;
    if (termp >= termn)
        W(:, i) = sqrt(S(i, i) * termp) * uup   / n_uup;
        H(i, :) = sqrt(S(i, i) * termp) * vvp.' / n_vvp;
    else
        W(:, i) = sqrt(S(i, i) * termn) * uun   / n_uun;
        H(i, :) = sqrt(S(i, i) * termn) * vvn.' / n_vvn;
    end
end
