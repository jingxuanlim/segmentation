function [W, H, V] = nmfh_lite_sparse(V, H, s, miniter, maxiter, tolfun, powr)
if ~exist('powr', 'var')
    powr = 1;
end
[n, t ] = size(V);
[k, t_] = size(H);
assert(t_ == t);

V = V ./ (mean(V.^powr, 2).^(1/powr));

dnorm_prev = [inf, inf];
for i = 1:maxiter
    H_ = H;
    
    W = double(V / H);
    W(W < 0) = 0;
    for j = 1:n
        W(j, :) = sparseness_projection(W(j, :), s(1  ));
    end
    
    H = double(W \ V);
    H(H < 0) = 0;    
    for j = 1:t
        H(:, j) = sparseness_projection(H(:, j), s(end));
    end
    H = H ./ (mean(H.^powr, 2).^(1/powr));
    
    dnorm = sqrt(mean2((V - W * H).^2));
    diffh = sqrt(mean2((H - H_   ).^2));
    if ((max(dnorm_prev) - dnorm)/dnorm < tolfun) && (diffh < tolfun)
        if i > miniter
            break
        end
    end
    dnorm_prev(2) = dnorm_prev(1);
    dnorm_prev(1) = dnorm;
    
    if any(any(isnan(W))) || any(any(isnan(H)))
        break;
    end
    
    fprintf('%4d\t%.8f\t%.8f\n', i, dnorm, diffh)
end
