function iter (a, i)
	i = i + 1
	local v = a[i]
	if v then
	  return i, v
	end
 end

 function ipairs (a)
	return iter, a, 0
 end

 a = {"one", "two", "three"}
 for i, v in ipairs(a) do
	print(i, v)
 end

 function pairs (t)
	return next, t, nil
 end

 print("--------pairs--------")
for k, v in pairs(a) do
 print(k,v)
end

print("-------next--------")
for k, v in next, a do
	print(k, v)
end
