include ../config.mk

XAMMAC_DLL = /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/XamMac.dll 
LIBXAMMAC_DLL = /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/libxammac.dylib 

DEMOS = \
	generate-pdf.exe	\
	locator.exe		\
	HelloCoreWlanSample.exe		

.PHONY: all clean

all: $(DEMOS)

clean:
	rm -f *.dll *.exe demo.pdf libxammac.dylib 

XamMac.dll: $(XAMMAC_DLL)
	cp "$<" $@

libxammac.dylib: $(LIBXAMMAC_DLL)
	cp "$<" $@

generate-pdf.exe: XamMac.dll libxammac.dylib generate-pdf.cs
	mcs generate-pdf.cs -r:XamMac.dll -r:System.Drawing

locator.exe: XamMac.dll libxammac.dylib locator.cs
	mcs locator.cs -r:XamMac.dll

HelloCoreWlanSample.exe: XamMac.dll libxammac.dylib HelloCoreWlanSample.cs
	mcs HelloCoreWlanSample.cs -r:XamMac.dll
