solution = []
max = 0;
for a = [0.15:0.05:0.3]
  for b = [0:0.05:0.3]
    for c = [0.15:0.05:0.3]
      dist = simulate(a, b, c, false);
      if dist > max
        max = dist;
        solution = [a, b, c];
      end
    end
  end
end
max
solution