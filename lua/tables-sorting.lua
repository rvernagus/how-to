local network = {
	{name = "grauna",  IP = "210.26.30.34"},
	{name = "arraial", IP = "210.26.30.23"},
	{name = "lua",     IP = "210.26.23.12"},
	{name = "derain",  IP = "210.26.23.20"},
}

for i, v in pairs(network) do
	print(i, v.name, v.IP)
end

table.sort(network, function (a,b)
	return (a.name < b.name)
end)

print("-----------------")
for i, v in pairs(network) do
	print(i, v.name, v.IP)
end
