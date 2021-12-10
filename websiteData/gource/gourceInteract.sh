# Generate a fresh log
rm -f gourceLog.txt
gource --output-custom-log gourceLog.txt
source gourceCustomize.sh

# Directly Visualize
gource	--1280x720 \
	--auto-skip-seconds 1 \
	--seconds-per-day 1.5 \
	--max-file-lag 1 \
	--file-idle-time 0 \
	--max-files 0 \
	--bloom-intensity 1.5 \
	--title "${projName}" --font-size 24 \
	--hide filenames,dirnames \
	--date-format "%B %d" \
	--multi-sampling \
	--caption-file gourceCaptions.txt \
	--caption-size 36 \
	--caption-duration 4 \
	gourcelog.txt