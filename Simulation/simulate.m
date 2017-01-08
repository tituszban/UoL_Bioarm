function v = simulate(l2, l_in, C_height, shw)
hold on;
axis equal;
dt = 0.0001;

t = 0;
sim_time = 0.1;



theta = 0;
phi = 0;
%phi = pi / 2 - (atan(l_in/30) + acos((l_in^2+l2^2)/(2*l2*sqrt(l_in^2+30^2))));

thetad = 0;
phid = 0;

thetadd = 0;
phidd = 0;

Bpr = 0;
[th, ph, Bpr] = tick(theta, phi, false, l2, l_in, C_height);

plt_time = sim_time / dt / 10;
plt_counter = plt_time;
max_dist = 0;

Blog = [];

th_max = 0;
ph_max = 0;

while t < sim_time && max_dist < 5
  plt = false;
  if plt_counter < plt_time
    plt_counter = plt_counter + 1;
  else
    plt = true;
    plt_counter = 0;
  end
  
  [thetadd, phidd, B] = tick(theta, phi, plt && shw, l2, l_in, C_height);
  thetad = thetad + thetadd * dt;
  phid = phid + phidd * dt;
  theta = theta + thetad * dt;
  theta = max([ min([theta, pi]), 0]);
  phi = phi + phid * dt;
  t = t + dt;
  
  Blog = [Blog; B];
  
  Bd = (B - Bpr)/dt;
  dist = calc_dist(B, Bd);
  if(dist > max_dist)
    max_dist = dist;
    th_max = theta;
    ph_max = phi;
  end
  
  Bpr = B;
end
[thetadd, phidd, B] = tick(theta, phi, shw, l2, l_in, C_height);

v = max_dist;
th_max / pi * 180;
ph_max / pi * 180;

%quiver(Blog(1, 1:size(Blog, 2) - 2), Blog(2, 1:size(Blog, 2) - 2), Blog(1, 2:size(Blog, 2)-1) - Blog(1, 1:size(Blog, 2) - 2), Blog(2, 2:size(Blog, 2)-1) - Blog(2, 1:size(Blog, 2) - 2))
plot(Blog(:, 1), Blog(:, 2));

hold off;