all:
	cp ../monomac.dll Hello.app/Contents/Resources/
	gmcs -debug -main:Demo hello.cs -out:Hello.app/Contents/Resources/Hello.exe -r:System.Drawing -r:Hello.app/Contents/Resources/monomac.dll 

run:
	./Hello.app/Contents/MacOS/Hello 
