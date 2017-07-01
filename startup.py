# -*- coding: utf-8 -*-

import numpy as np
import sys,os,os.path
import array as ar
import struct as st
import scipy.io as sio
import math
import matplotlib
import matplotlib.pyplot as plt
import pylab
from skimage.external.tifffile import imread, imsave
sys.path.append(os.path.expanduser('C:\\Users\\kawashimat\\Documents\\Spyder\\functions'))
sys.path.append(os.path.expanduser('C:\\Users\\kawashimat\\Documents\\Spyder\\nmf_segment'))
import sys,os,os.path
os.environ['KERAS_BACKEND']='theano'

matplotlib.rcParams['pdf.fonttype'] = 42

def clear_all():
    """Clears all the variables from the workspace of the spyder application."""
    gl = globals().copy()
    for var in gl:
        if var[0] == '_': continue
        if 'func' in str(globals()[var]): continue
        if 'module' in str(globals()[var]): continue
        if 'autocall' in str(globals()[var]): continue
        if 'method' in str(globals()[var]): continue
        if 'InteractiveShell' in str(globals()[var]): continue
    
        del globals()[var]
        del var
        