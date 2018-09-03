function [B, H] = bandpass_filter(T, f, f_rng, order, tip)

% inputs:
%   T : timeseries vector
%   f : sampling frequency (in Hz)
%   fl: low cut-off (0 if empty or -inf)
%   fh: high cut-off (nyquist if empty or inf)
%
% output:
%   F:  filtered timeseries
%   H:  hilbert transformed timeseries
%   plot frequency vs power as: plot(F,P)

nyquist = f/2;                                              % nyquist frequency

if ~exist('order', 'var') || isempty(order)
    order = 1000;                                           % the more, the better
end
if ~exist('tip', 'var') || isempty(tip)
    tip = 'fir';
end

f_rng(1) = max(f_rng(1), 1e-10);
f_rng(2) = min(f_rng(2), nyquist-1e-10);

switch tip
    case 'fir',     krnl = fir1(  order, f_rng/nyquist);   	% FIR filter
    case 'butter',  krnl = butter(order, f_rng/nyquist);   	% butterworth filter
end

B = filtfilt(krnl, 1, double(T));                           % bandpass filter
H = abs(hilbert(B));                                        % get hilbert transform
