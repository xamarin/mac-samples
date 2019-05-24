run:: obj/Test.app/
	obj/Test.app/GuiUnit.app/Contents/MacOS/GuiUnit -noheader `pwd`/obj/Test.dll

clean::
	rm -rf ./obj/

obj/Test.app/: obj/ obj/Test.dll obj/GuiUnit.exe
	/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin/mmp --output obj/Test.app obj/GuiUnit.exe -a /Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll --nolink --profile mobile --arch x86_64

obj/:
	mkdir obj

obj/Test.dll: obj/ obj/GuiUnit.exe Test.cs
	mcs -out:obj/Test.dll -t:library -debug -r:obj/GuiUnit.exe -r:/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll Test.cs

obj/guiunit/: obj/
	if [ ! -d "obj/guiunit" ]; then git clone https://github.com/mono/guiunit.git obj/guiunit; fi

obj/GuiUnit.exe: obj/ obj/guiunit/
	msbuild obj/guiunit/src/framework/GuiUnit_NET_4_5.csproj
	cp obj/guiunit/bin/net_4_5/GuiUnit.exe obj/
