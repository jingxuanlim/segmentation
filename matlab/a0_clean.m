output_dir = [pwd '/'];
imageframe_nmbr = h5read([output_dir 'prepro_parameters.hdf5'], '/imageframe_nmbr');
freq = h5read([output_dir 'prepro_parameters.hdf5'], '/freq_stack');

try
    dt_range = h5read('prepro_parameters.hdf5', '/dt_range') + 1;
catch
    dt_range = 1:h5read('prepro_parameters.hdf5', '/lt');
end

freq_lims = [0.01, 0.1];
thr_similarity = 0.5;

%%

for frame_i = 0:imageframe_nmbr-1
    
    filename = [output_dir 'Cells' num2str(frame_i) '.hdf5'];
    
    dims = double(h5read(filename, '/dims'));
    x = dims(1);
    y = dims(2);
    z = dims(3);
    t = dims(4);
    
    if (t > 2e4 && freq < 10) || (~isfinite(freq))
        freq = input(sprintf('t: %d, freq: %.3f. Enter frequency: ', t, freq));
        fprintf('Continuing with frequency: %f\n', freq);
    end
    
    Cmpn_position = h5read(filename, '/Cell_position') + 1;
    Cmpn_X = squeeze(Cmpn_position(1, :, :)).';
    Cmpn_Y = squeeze(Cmpn_position(2, :, :)).';
    Cmpn_Z = squeeze(Cmpn_position(3, :, :)).';
    Cmpn_spcesers = h5read(filename, '/Cell_spcesers').';
    Cmpn_timesers = h5read(filename, '/Cell_timesers').';
    
    %%
    
    for rn = 1:0
        ix = find(any(Cmpn_timesers < 0.25, 1));
        if numel(ix)
            ix0 = ix;
            disp(['mins: ' num2str(numel(ix)) '; filter range: ' num2str(rn)]);
            for i = 1:numel(ix)
                lb = max(ix(i)-rn, 1);
                ub = min(ix(i)+rn, t);
                Cmpn_timesers(:, ix(i)) = median(Cmpn_timesers(:, lb:ub), 2);
            end
        else
            break
        end
    end
    
    %%
    
    ix = any(isnan(Cmpn_timesers), 2);
    if nnz(ix)
        disp(['nans: ' num2str(nnz(ix))]);
        Cmpn_timesers(ix,:) = rand(nnz(ix), t) * min(nonzeros(Cmpn_timesers));
    end
    
    %%
    
    Cmpn_bandpowr = log10(bandpower(Cmpn_timesers(:, dt_range).', freq, freq_lims)).';
    if nnz(ix)
        Cmpn_bandpowr(ix, :) = min(Cmpn_bandpowr(~ix, :));
    end
    
    gmm = fitgmdist(Cmpn_bandpowr, 2, 'replicates', 100, 'options', statset('maxiter', 1000));
    
    Cmpn_signalpr = posterior(gmm, Cmpn_bandpowr);
    [~, idxm] = max(Cmpn_signalpr);
    [~, idxb] = max(Cmpn_bandpowr(idxm));
    Cmpn_signalpr = Cmpn_signalpr(:, idxb);
    
    flag_thr_powr = 0;
    while ~flag_thr_powr
        figure(1),
        subplot(121), hist(Cmpn_bandpowr, 100); axis square
        title('Component timeseries power histogram')
        subplot(122), hist(Cmpn_signalpr, 100); axis square
        title('Signal vs noise probability threshold')
        thr_prob = input('Enter signal probability threshold: ');
        [~, ix] = min(abs(Cmpn_signalpr - thr_prob));
        thr_powr = Cmpn_bandpowr(ix);
        if isinf(thr_prob)
            thr_powr = thr_prob;
        end
        Cell_validity = (Cmpn_signalpr > thr_prob) & (Cmpn_bandpowr > thr_powr);
        
        IDX = [{find(Cell_validity).'} {find(~Cell_validity).'}];
        
        show_vol(IDX, Cmpn_X, Cmpn_Y, Cmpn_Z, x, y, z, ...
            Cmpn_spcesers, Cmpn_bandpowr, Cmpn_signalpr)
        
        flag_thr_powr = ...
            input(['Is thr_signalprob = ' num2str(thr_prob) ' accurate? [1, yes]; 0, no. ']);
    end
    close
    
    %%
    
    % Cmpn_maxmin = max(Cmpn_timesers, [], 2) .* min(Cmpn_timesers, [], 2);
    %
    % figure(1), clf
    % subplot(121), hist(Cmpn_maxmin, 100); axis square
    % subplot(122), hist(Cmpn_maxmin .* (Cmpn_maxmin > 1), 100); axis square
    %
    % IDX = [{find(Cell_validity & (Cmpn_maxmin >  1)).'} ...
    %        {find(Cell_validity & (Cmpn_maxmin <= 1)).'}];
    %
    % show_vol(IDX, Cmpn_X, Cmpn_Y, Cmpn_Z, x, y, z, ...
    %    Cmpn_spcesers, Cmpn_bandpowr, Cmpn_signalpr);
    %
    % flag_thr_maxm = input(['Remove ' num2str(nnz(Cell_validity & Cmpn_maxmin<=1)) ...
    %     ' components with maxmin <= 1? [1, yes]; 0, no. ']);
    % if isempty(flag_thr_maxm) || flag_thr_maxm
    %     thr_maxm = 1;
    %     Cell_validity = Cell_validity & (Cmpn_maxmin > thr_maxm);
    % end
    
    %%
    
    L = cell(x, y, z);
    for i = find(Cell_validity).'
        for j = find(isfinite(Cmpn_X(i,:)))
            L{Cmpn_X(i, j), Cmpn_Y(i, j), Cmpn_Z(i, j)} = ...
                [i L{Cmpn_X(i, j), Cmpn_Y(i, j), Cmpn_Z(i, j)}];
        end
    end
    
    N = cellfun(@numel, L);
    olap_pairs = zeros(sum(sum(sum(N .* (N-1) / 2))), 2);
    
    h = 0;
    for zi = 1:z
        for yi = 1:y
            for xi = 1:x
                li = L{xi, yi, zi};
                for u = 1:numel(li)
                    for v = u + 1:numel(li)
                        h = h + 1;
                        olap_pairs(h, :) = sort([li(u), li(v)]);
                    end
                end
            end
        end
    end
    
    [uolap_pairs, idxu] = unique(sortrows(olap_pairs), 'rows', 'last');
    size_uolap_pairs = diff([0; idxu]);
    size_cell = sum(Cmpn_spcesers > 0, 2);
    spatial_similarity_uolap_pairs = size_uolap_pairs ./ mean(size_cell(uolap_pairs), 2);
    
    hiolap_pairs = uolap_pairs(spatial_similarity_uolap_pairs > thr_similarity, :);
    corr_hiolap_pairs = zeros(size(hiolap_pairs, 1), 1);
    for i = 1:size(corr_hiolap_pairs,1)
        time_hiolap_pair = Cmpn_timesers(hiolap_pairs(i,:), :);
        corr_hiolap_pairs(i) = corr(time_hiolap_pair(1,:).', time_hiolap_pair(2,:).');
    end
    duplicate_pairs = hiolap_pairs(corr_hiolap_pairs > thr_similarity, :);
    
    Cmpn_wsum = nansum(Cmpn_spcesers, 2);
    for i = 1:size(duplicate_pairs, 1)
        [~, idxm] = min(Cmpn_wsum(duplicate_pairs(i, :)));
        Cell_validity(duplicate_pairs(i, idxm)) = 0;
    end
    
    %%
    
    n = nnz(Cell_validity);
    
    Cell_X = Cmpn_X(Cell_validity, :);
    Cell_Y = Cmpn_Y(Cell_validity, :);
    Cell_Z = Cmpn_Z(Cell_validity, :);
    
    Cell_spcesers = Cmpn_spcesers(Cell_validity, :);
    Cell_timesers = Cmpn_timesers(Cell_validity, :);
    
    Volume = zeros(x, y, z);
    Labels = zeros(x, y, z);
    for i = 1:size(Cell_X, 1)
        for j = 1:nnz(isfinite(Cell_X(i, :)))
            xij = Cell_X(i, j); yij = Cell_Y(i, j); zij = Cell_Z(i, j);
            if Cell_spcesers(i, j) > Volume(xij, yij, zij)
                Volume(xij, yij, zij) = Cell_spcesers(i, j);
                Labels(xij, yij, zij) = i;
            end
        end
    end
    
    save([output_dir 'Cells' num2str(frame_i) '_clean'], '-v7.3',   ...
        'n', 'x', 'y', 'z', 'freq', 'Cell_X', 'Cell_Y', 'Cell_Z',           ...
        'Cell_spcesers', 'Cell_timesers', 'Volume', 'Labels');
    
end


function show_vol(IDX, Cmpn_X, Cmpn_Y, Cmpn_Z, x, y, z, ...
    Cmpn_spcesers, Cmpn_sgnlfeat, Cmpn_sgnlprob)
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
cmax = prctile(SNV{1}(:), 99);

figure(1), clf
subplot(221), hist(Cmpn_sgnlfeat(IDX{1}), 100); axis square
title('Component timeseries power histogram')
subplot(222), hist(Cmpn_sgnlprob(IDX{1}), 100); axis square
title('Signal vs noise probability threshold')
for i = 1:z
    subplot(223), imagesc(SNV{1}(:,:,i).',[0, cmax]);
    colormap hot; axis equal
    title([num2str(numel(IDX{1})) ' cells to be kept.'])
    subplot(224), imagesc(SNV{2}(:,:,i).',[0, cmax]);
    colormap hot; axis equal
    title([num2str(numel(IDX{2})) ' cells to be deleted.'])
    pause;
end

subplot(223), imagesc(max(SNV{1},[],3).',[0, cmax]);
colormap hot; axis equal
title([num2str(numel(IDX{1})) ' cells to be kept.'])
subplot(224), imagesc(max(SNV{2},[],3).',[0, cmax]);
colormap hot; axis equal
title([num2str(numel(IDX{2})) ' cells to be deleted.'])

end