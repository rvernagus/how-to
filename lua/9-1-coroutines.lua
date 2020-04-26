local co = coroutine.create(function()
print("hi")
end)

print(co)

-- coroutines can be in three statuses: suspended, running, dead
print(coroutine.status(co))

co = coroutine.create(function ()
	for i = 1, 3 do
		print("co", i)
		coroutine.yield()
	end
end)

coroutine.resume(co)
print(coroutine.status(co))
coroutine.resume(co)
coroutine.resume(co)
coroutine.resume(co)
print(coroutine.resume(co))

co = coroutine.create(function (a,b)
	coroutine.yield(a + b, a - b)
end)
print(coroutine.resume(co, 20, 10))  --> true  30  10

co = coroutine.create (function ()
	print("co", coroutine.yield())
end)
coroutine.resume(co)
coroutine.resume(co, 4, 5)     --> co  4  5
