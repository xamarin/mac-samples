DEMOS = \
	generate-pdf.exe	\
	locator.exe

all: $(DEMOS)
	MONO_PATH=../../src/ mono generate-pdf.exe

generate-pdf.exe: generate-pdf.cs
	gmcs generate-pdf.cs -r:../../src/MonoMac.dll -r:System.Drawing

locator.exe: locator.cs
	gmcs locator.cs -r:../../src/MonoMac.dll 