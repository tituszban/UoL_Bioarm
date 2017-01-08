function [thetadd, phidd, Bpos] = tick(theta, phi, plt, time)

box_section_linear_density = 0.297633;  %kg/m, 0.2 lbs / ft, (source: https://www.metalsdepot.com/products/alum2.phtml?page=tube)
box_section_width = 0.0127;             %m

r1k = 87.65; %N/m, 2 rubber bands in parallel
r1l = 0.16;  %m
r2k = 71.71; %N/m, (2 rubber bands in parallel) in series with (2 double bands in parallel)
r2l = 0.238; %m

l1 = 0.3;   %m
l2 = 0.32;   %m
lB = 0.32;   %m

lr2 = 0.01;  %m

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

O = [0.0, 0, 0];
C = [0.5, 0.3, 0];
A = O + [cos(theta) * l1, sin(theta) * l1, 0];
B = A + [-cos(phi) * lB, sin(phi) * lB, 0];
D = A + [-cos(phi) * lr2, sin(phi) * lr2, 0];
E = O + [cos(theta) * l1 / 2, sin(theta) * l1 / 2, 0];
F = A + [-cos(phi) * l2 / 2, sin(phi) * l2 / 2, 0];
G = A + [-cos(phi) * l2, sin(phi) * l2, 0];

Fk1 = rubber_force(A, C, r1k, r1l);
Fk2 = rubber_force(D, C, r2k, r2l);

MO = cross(E-O, Fg1) + cross(A-O, Fk1) + cross(F-O, Fg2) + cross(D-O, Fk2) + cross(B-O, FgB);
MA = cross(F-A, Fg2) + cross(D-A, Fk2) + cross(B-A, FgB);

IO = I1 + m1 * (l1 / 2)^2 + I2 + m2 * dist(O, F)^2 + IB + mB * dist(O, B)^2;
IA = I2 + m2 * (l2/2)^2 + IB + mB * lB^2;

thetadd = sum( MO / IO );
phidd = sum( MA / IA );

Bpos = B;

if plt
  p = [O; E; A; F; D; B; G];
  plot(p(:,1), p(:, 2), 'bo-');
  hold on;
  clabel(time);
  plot(C(1), C(2))
  plot([C(1); A(1)], [C(2); A(2)], 'r--');
  plot([C(1); D(1)], [C(2); D(2)], 'g--');
end


simple catapult:


function [thetadd, phidd, Bpos] = tick(theta, phi, plt, time)

box_section_linear_density = 0.297633;  %kg/m, 0.2 lbs / ft, (source: https://www.metalsdepot.com/products/alum2.phtml?page=tube)
box_section_width = 0.0127;             %m

r1k = 256.704; %N/m, 2 rubber bands in parallel
r1l = 0.16;  %m

l1 = 0.5;   %m
lB = 0.3;   %m

lr2 = 0.3;  %m

g = [0, -9.81, 0];  %m/s^2

mB = 0.2;   %kg
rB = 0.08;  %m

m1 = l1 * box_section_linear_density;

I1 = (m1 * (box_section_width^2 + l1^2))/12;
IB = (2 * mB * rB ^ 2)/5;

Fg1 = g * m1;
FgB = g * mB;

O = [0.5, 0, 0];
C = [0.5, 0.3, 0];
A = O + [cos(theta) * l1, sin(theta) * l1, 0];
D = O + [cos(theta) * lr2, sin(theta) * lr2, 0];
B = A;
E = O + [cos(theta) * l1 / 2, sin(theta) * l1 / 2, 0];

Fk1 = rubber_force(D, C, r1k, r1l);

MO = cross(E-O, Fg1) + cross(A-O, Fk1) + cross(B-O, FgB);

IO = I1 + m1 * (l1 / 2)^2 + IB + mB * dist(O, B)^2;

thetadd = sum( MO / IO );
phidd = 0;

Bpos = B;

if plt
  p = [O; D; A; B];
  plot(p(:,1), p(:, 2), 'bo-');
  hold on;
  clabel(time);
  plot(C(1), C(2), '*')
  plot([C(1); D(1)], [C(2); D(2)], 'r--');
end