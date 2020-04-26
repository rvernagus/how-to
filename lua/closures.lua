local function newCounter()
	local i = 0
	return function ()
		i = i + 1
		return i
	end
end

local c1 = newCounter()
local c2 = newCounter()
print(c1())
print(c1())
print(c2())
print(c1())
print(c2())

print("--------Wrap and Redfine Existing Functions With A Closure----------")
print(math.sin(1))
do
	local oldSin = math.sin
	local k = math.pi / 180
	math.sin = function (x)
		return oldSin(x*k)
	end
end
print(math.sin(1))

-- Be careful with recursive closeres, do this way:
local function fact (n)
	if n == 0 then return 1
	else return n*fact(n-1)
	end
 end
