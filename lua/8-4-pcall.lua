local throwException = true

local function foo()
	-- some work
	if throwException then error() end
end

if pcall(foo) then
	-- no errors while running foo
	print("All's good")
else
	-- foo raised an error, take action
	print("Oops, an error occurred!")
end

-- Or use an anonymous function
if pcall(function()
	foo()
end) then
	print("All's good")
else
	print("Oops, an error occurred!")
end

-- Error doesn't have to be a string
local status, err = pcall(function() error({code=121}) end)
print(err.code)
