function show_vol(IDX, Cmpn_X, Cmpn_Y, Cmpn_Z, x, y, z, ...
    Cmpn_spcesers, Cmpn_sgnlfeat, Cmpn_sgnlprob)

if isempty(x); x = max(Cmpn_X(:)); end
if isempty(y); y = max(Cmpn_Y(:)); end
if isempty(z); z = max(Cmpn_Z(:)); end

SNV = cell(1,2);
for h = 1:2
    SNV{h} = zeros(x, y, z);
    for i = IDX{h}
        for j = 1:nnz(isfinite(Cmpn_X(i, :)))
            xij = Cmpn_X(i,j); yij = Cmpn_Y(i,j); zij = Cmpn_Z(i,j);
            SNV{h}(xij, yij, zij) = max(SNV{h}(xij, yij, zij), Cmpn_spcesers(i, j));
        end
    end
end

figure(1), clf
subplot(221), hist(Cmpn_sgnlfeat(IDX{1}), 100); axis square
title('Component timeseries power histogram')
subplot(222), hist(Cmpn_sgnlprob(IDX{1}), 100); axis square
title('Signal vs noise probability threshold')
subplot(223), imagesc(max(SNV{1},[],3).',[0, 0.05]); colormap hot; axis equal
title([num2str(numel(IDX{1})) ' cells to be kept.'])
subplot(224), imagesc(max(SNV{2},[],3).',[0, 0.05]); colormap hot; axis equal
title([num2str(numel(IDX{2})) ' cells to be deleted.'])
end
