local status, err = pcall(function() a = 'a' + 1 end)
print(err)

-- Second argument can blame error on caller
local function foo(str)
	if type(str) ~= "string" then
		error("string expected", 2)
	end
end

status, err = pcall(function() foo({x=1}) end) -- prints this line as source of error
print(err)

print(debug.traceback())
