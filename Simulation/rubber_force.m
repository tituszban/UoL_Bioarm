function F = rubber_force(A, C, k, l)

R = C - A;
a = R(1);
b = R(2);
c = hypot(a, b);

f = 0;
if c > l
  f = (c - l) * k;
else
  f = 0;
end

F = R/norm(R) * f;