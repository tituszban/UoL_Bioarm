function d = calc_dist(P, V)
g = 9.81;

p = [-g / 2, V(2), P(2)];
t = max(roots(p));

d = P(1) + V(1) * t;