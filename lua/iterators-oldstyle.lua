local function myiter (f)
	local a = {"one", "two", "three"}
	for _, item in next, a do
		f(item)
	end
end

myiter(print)
