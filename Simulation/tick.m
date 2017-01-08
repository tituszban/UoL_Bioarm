function [thetadd, phidd, Bpos] = tick(theta, phi, plt, l2_p, l_in_p, C_height_p)

box_section_linear_density = 0.297633;  %kg/m, 0.2 lbs / ft, (source: https://www.metalsdepot.com/products/alum2.phtml?page=tube)
box_section_width = 0.0127;             %m

r1k = 42.78 * 1; % 87.65 / 2; %N/m, 2 rubber bands in parallel
r1l = 0.16;  %m
r2k = 213.92;% 93.05; % 97.62; % 72.14; %N/m, (2 rubber bands in parallel) in series with (2 double bands in parallel)
r2l = 0.16; %0.238; %m

l1 = 0.3;   %m
l2 = 0.5;   %m
lB = 0.42;   %m

lr2 = l2_p;  %m
lr1 = 0.3;  %m

g = [0, -9.81, 0];  %m/s^2

mB = 0.2;   %kg
rB = 0.08;  %m

m1 = l1 * box_section_linear_density * 2;
m2 = l2 * box_section_linear_density;

I1 = (m1 * (box_section_width^2 + l1^2))/12;
I2 = (m2 * (box_section_width^2 + l2^2))/12;
IB = (2 * mB * rB ^ 2)/5;

Fg1 = g * m1;
Fg2 = g * m2;
FgB = g * mB;

O = [0.2, 0, 0];        %base pivot
C = [0.5, C_height_p, 0];      %rubber 1 fix
H = [0.5-l_in_p, 0.3, 0];      %rubber 2 fis
A = O + [cos(theta) * l1, sin(theta) * l1, 0];          %end of arm 1
J = O + [cos(theta) * lr1, sin(theta) * lr1, 0];        %rubber 1 position on arm 1
B = A + [-cos(phi) * lB, sin(phi) * lB, 0];             %ball position on arm 2
D = A + [-cos(phi) * lr2, sin(phi) * lr2, 0];           %rubber 2 position on arm 2
E = O + [cos(theta) * l1 / 2, sin(theta) * l1 / 2, 0];  %arm 1 CofM
F = A + [-cos(phi) * l2 / 2, sin(phi) * l2 / 2, 0];     %arm 2 CofM
G = A + [-cos(phi) * l2, sin(phi) * l2, 0];             %end of arm 2

Fk1 = rubber_force(J, C, r1k, r1l);
Fk2 = rubber_force(D, H, r2k, r2l);

MO = cross(E-O, Fg1) + cross(J-O, Fk1) + cross(F-O, Fg2) + cross(D-O, Fk2) + cross(B-O, FgB);
MA = cross(F-A, Fg2) + cross(D-A, -Fk2) + cross(B-A, FgB);

IO = I1 + m1 * (l1 / 2)^2 + I2 + m2 * dist(O, F)^2 + IB + mB * dist(O, B)^2;
IA = I2 + m2 * (l2/2)^2 + IB + mB * lB^2;

thetadd = sum( MO / IO );
phidd = sum( MA / IA );

Bpos = B;

if plt
  p = [O; E; J; A; F; D; B; G];
  plot(p(:,1), p(:, 2), 'bo-');
  plot(C(1), C(2))
  plot(H(1), H(2))
  plot([C(1); J(1)], [C(2); J(2)], 'r--');
  plot([H(1); D(1)], [H(2); D(2)], 'g--');
end