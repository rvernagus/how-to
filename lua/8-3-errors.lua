print("Enter a number: ")
n = io.read("*number")
if not n then error("Invalid input") end

-- This is the same as the above
print("Enter a number: ")
n = assert(io.read("*number"), "Invalid input")

local file, msg
repeat
	print("Enter a file name:")
	local name = io.read()
	if not name then return end
	file, msg = io.open(name, "r")
	if not file then print(msg) end
until file

print("Enter a file name:")
local name = io.read()
local file = assert(io.open(name, "r"))
