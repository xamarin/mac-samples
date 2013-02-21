MDROOT = $(shell stat -f%N "/Applications/Xamarin Studio.app" 2>/dev/null || echo "/Applications/MonoDevelop.app")
MDROOT_ESCAPED = $(shell echo "$(MDROOT)" | sed 's| |\\ |g')
MDTOOL = $(MDROOT)/Contents/MacOS/mdtool
