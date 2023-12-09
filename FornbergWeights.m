function [c] = weights(z, x, m, h)
%WEIGHTS(z, x, n, nd, m, h) Computes the optimal weights for discrete 
%   derivative approximations of arbitrary order, location, and grid 
%   spacing.  A port of a FORTRAN code published by Fornberg in 1998; 
%   his description as follows.
% 
%   Calculation of Weights in Finite Difference Formulas
%   Bengt Fornberg
%   SIAM Review
%   Vol. 40, No. 3 (Sep., 1998), pp. 685-691
%   Published by: Society for Industrial and Applied Mathematics
%   Stable URL: http://www.jstor.org/stable/2653239
%
%INPUT PARAMETERS
%
%   z           location where approximations are to be accurate
%
%   x(0:nd)     grid point locations, found in x(0:n)
%
%   m           highest derivative for which weights are sought
%
%   h           human readable output.  Causes only the mth order to be
%                  printed, but will be printed in rational form. Added by
%                  Charles Cook.
%
%OUTPUT PARAMETERS
%
%       c(0:nd,0:m) weights at grid locations x(0:n) for derivatives of
%                       order 0:m, found in c(0:n,0:m)
%
%EXAMPLE
%
%       weights(0,[-2 -1 0 1 2], 4, 4, 2)
%       Finds centered approximation for second order derivative of fourth
%       order.  Grab the last column for the coefficients.
%
%       weights(0.5,[-2, -1, 0, 1, 2], 0, 1)
%       Interpolate the value at 0.5, with values from -2, -1, 0, 1, and 2.
%
%CHANGE LOG
%
%   - Ported to MATLAB, C. Cook
%   - Added 'human' output, C. Cook
%   - Removed unecessary dimensioning arguements, C. Cook

    % one less than the total number of grid points; n must
    % not exceed the parameter nd below
    n = length(x)-1;
    
    % dimension of x- and c-arrays in calling program x(0:nd)
    % and c(0:nd,0:m), respectively
    nd = n;
    
    c = vpa(zeros(nd+1,m+1));
    c1 = 1;
    c4 = x(1) -z;
    c(1,1) = 1;
    
    for i=1:n
        mn = min(i,m);
        c2 = 1;
        c5 = c4;
        c4 = x(i+1)-z;
        for j=0:i-1
            c3 = x(i+1)-x(j+1);
            c2 = c2*c3;
            if (j == i-1)
                for k=mn:-1:1
                    c(i+1,k+1) = c1*(k*c(i,k)-c5*c(i,k+1))/c2;
                end
                c(i+1,1) = -c1*c5*c(i,1)/c2;
            end
            for k = mn:-1:1
                c(j+1,k+1) = (c4*c(j+1,k+1)-k*c(j+1,k))/c3;
            end
            c(j+1,1) = c4*c(j+1,1)/c3;
        end
        c1 = c2;
    end
    if nargin == 4
        if (h)
            c = c(:,end);
            c = rats(c);
        end
    end
end