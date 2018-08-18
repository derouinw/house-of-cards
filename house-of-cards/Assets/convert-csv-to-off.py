# the offizer

def file_len(fname):
    with open(fname) as f:
        for i, l in enumerate(f):
            pass
    return i + 1

def convert_csv_to_off(fname):
	print("Converting " + fname)
	length = file_len(fname)
	new_name = str(i) + new_extension

	with open(new_name) as f:
		f.write("COFF\n")
		f.write(str(length) + "\n")

		with open(old_name) as old:
			for line in old:
				f.write(line)

start_index = 1
end_index = 2101
extension = ".csv"
new_extension = ".off"

for i in xrange(start_index, end_index):
	old_name = str(i) + extension

	convert_csv_to_off(old_name)

convert_csv_to_off("city.csv")
convert_csv_to_off("culdesac.csv")
