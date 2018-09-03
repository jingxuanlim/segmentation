for frame_i in range(imageframe_nmbr):

    thr_signalprob_default = 0.5
    thr_similarity = 0.5
    freq_lims = (0.01, 0.1)

    with h5py.File(output_dir + 'Cells' + str(frame_i) + '.hdf5', 'r') as file_handle:
        Cell_position = file_handle['Cell_position'][()]
        Cell_spcesers = file_handle['Cell_spcesers'][()]
        Cell_timesers = file_handle['Cell_timesers'][()]
        freq = file_handle['freq'][()]

    # convert additional statistics and convert to arrays
    cn = len(Cell_position)
    Cell_wsum = np.nansum(Cell_spcesers)

    freq, Cell_pwsd = signal.periodogram(Cell_timesers, 1000 / t_stack, axis=1)

    lidx0 = (freq > freq_lims[0]) & (freq < freq_lims[1])
    Cell_band = np.log10(np.sum(Cell_pwsd[:, lidx0], 1))[:, None]
    gmm = mixture.GMM(n_components=2, n_iter=100, n_init=100).fit(Cell_band)
    Cell_signalpr = gmm.predict_proba(Cell_band)
    Cell_signalpr = Cell_signalpr[:, np.argmax(Cell_band[np.argmax(Cell_signalpr, 0)])]

    thrs_flag = 0
    while thrs_flag == 0:
        pp.figure(1, (12, 4))
        pp.subplot(121);
        _ = pp.hist(Cell_band, 100);
        xlim1 = pp.xlim()
        pp.subplot(122);
        _ = pp.hist(Cell_signalpr, 100)
        xlim2 = pp.xlim()
        pp.show()

        try:
            thr_signalprob = input('Enter signal probability threshold: ')
        except SyntaxError:
            thr_signalprob = thr_signalprob_default

        Cell_validity = (Cell_signalpr > thr_signalprob)

        pp.figure(1, (12, 4))
        pp.subplot(121);
        _ = pp.hist(Cell_band[Cell_validity], 100)
        pp.xlim(xlim1)
        pp.subplot(122);
        _ = pp.hist(Cell_signalpr[Cell_validity], 100)
        pp.xlim(xlim2)
        pp.title(str(np.count_nonzero(Cell_validity)) + ' valid cells.')
        pp.show()

        try:
            thrs_flag = input('Is thr_signalprob=' + str(thr_signalprob) + ' accurate? [1, yes]; 0, no. ')
        except SyntaxError:
            thrs_flag = 1

    # brain mask array
    L = [[[[] for zi in xrange(lz)] for yi in xrange(ly//ds)] for xi in xrange(lx//ds)]
    for i in xrange(cn):
        if Cell_validity[i]:
            xyzi = Cell_position[i]
            for xi, yi, zi in xyzi:
                L[xi][yi][zi].append(i)

    def get_pairs(xyz, l):
        return [[xyz[i], xyz[j]] for i in xrange(l) for j in xrange(i + 1, l)]

    vox_pairs = [get_pairs(xyz, len(xyz)) for X in L for Y in X for xyz in Y if len(xyz) > 1]
    all_pairs = np.sort([p for pairs in vox_pairs for p in pairs], 1)
    _, unique_idx = np.unique(all_pairs[:, 0] + 1j * all_pairs[:, 1], return_index=True)
    unique_pairs = all_pairs[unique_idx]

    def get_spatial_similarity((A, B)):
        d = [np.all(a == b) for a in A for b in B]
        return 2.0 * np.sum(d) / (len(A) + len(B))

    spatial_similarity_unique_pairs = \
        sc.parallelize([(Cell_position[p[0]], Cell_position[p[1]]) for p in unique_pairs])\
        .map(get_spatial_similarity)\
        .collect()
    spatial_similarity_unique_pairs = np.array(spatial_similarity_unique_pairs)

    overlap_pairs = unique_pairs[spatial_similarity_unique_pairs > thr_similarity]
    pearson_correlation_overlap_pairs = \
        sc.parallelize([(Cell_timesers[p[0]], Cell_timesers[p[1]]) for p in overlap_pairs])\
        .map(lambda (A, B): stats.pearsonr(A, B)[0])\
        .collect()
    pearson_correlation_overlap_pairs = np.array(pearson_correlation_overlap_pairs)
    duplicate_pairs = overlap_pairs[pearson_correlation_overlap_pairs > thr_similarity]

    colon = xrange(len(duplicate_pairs))
    redundancy = np.argmin(Cell_wsum[duplicate_pairs], 1)
    Cell_validity[duplicate_pairs[colon, redundancy]] = 0

    W_signalpr = np.zeros((lx//ds, ly//ds, lz), dtype='float32')
    for cell_i in xrange(cn):
        W_signalpr[zip(*Cell_position[cell_i])] = Cell_signalpr[cell_i]
        
    Cell_timesers1, Cell_baseline1 = \
        list(zip(*sc.parallelize(Cell_timesers).map(detrend_dynamic_baseline).collect()))

    with h5py.File(output_dir + 'Cells' + str(frame_i) + '.hdf5', 'r+') as filename:
        for i, cell_idx in enumerate(np.nonzero(Cell_validity)[0]):
            hdf5_dir = '/Cell/' + str(i).zfill(6)
            filename[hdf5_dir + '/Cell_position1'] = Cell_position[cell_idx]
            filename[hdf5_dir + '/Cell_spcesers1'] = Cell_spcesers[cell_idx]
            filename[hdf5_dir + '/Cell_timesers1'] = Cell_timesers1[cell_idx]
            filename[hdf5_dir + '/Cell_baseline1'] = Cell_baseline1[cell_idx]
