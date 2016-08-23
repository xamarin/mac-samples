include ../config.mk

XM_DIR = /Library/Frameworks/Xamarin.Mac.framework
XAMMAC_DLL = $(XM_DIR)/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll
LIBXAMMAC_DLL = /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/libxammac.dylib 

DEMOS = \
	generate-pdf.app \
	locator.app \
	HelloCoreWlanSample.app

.PHONY: all clean

all: $(DEMOS)

clean:
	rm -fr obj/
	rm -f demo.pdf 
	rm -fr generate-pdf.app/
	rm -fr locator.app/
	rm -fr HelloCoreWlanSample.app/
	rm -f generate-pdf
	rm -f locator
	rm -f HelloCoreWlanSample

obj/:
	mkdir -p obj

generate-pdf:
	echo "./generate-pdf.app/Contents/MacOS/generate-pdf" > generate-pdf
	chmod +x generate-pdf

locator:
	echo "./locator.app/Contents/MacOS/locator" > locator
	chmod +x locator

HelloCoreWlanSample:
	echo "./HelloCoreWlanSample.app/Contents/MacOS/HelloCoreWlanSample" > HelloCoreWlanSample
	chmod +x HelloCoreWlanSample

generate-pdf.app: generate-pdf.cs obj/ generate-pdf
	mcs generate-pdf.cs -r:$(XAMMAC_DLL) -out:obj/generate-pdf.exe
	$(XM_DIR)/Commands/mmp  obj/generate-pdf.exe --target-framework=Mobile

locator.app: locator.cs obj/ locator
	mcs locator.cs -r:$(XAMMAC_DLL) -out:obj/locator.exe
	$(XM_DIR)/Commands/mmp  obj/locator.exe --target-framework=Mobile

HelloCoreWlanSample.app: HelloCoreWlanSample.cs obj/ HelloCoreWlanSample
	mcs HelloCoreWlanSample.cs -r:$(XAMMAC_DLL) -out:obj/HelloCoreWlanSample.exe
	$(XM_DIR)/Commands/mmp  obj/HelloCoreWlanSample.exe --target-framework=Mobile
