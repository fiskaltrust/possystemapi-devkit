#!/bin/bash

####################################################################################
### dependencies
#
# install pandock: sudo apt install pandoc texlive-latex-base texlive-latex-recommended librsvg2-bin
# install other dependencies: sudo apt install zip
#
# RESULT: creates a ZIP package in build/out/pos-system-api-devkit-package.zip
####################################################################################

DIR="$(cd "$(dirname "$(readlink -f "$0")")" && pwd)"
BUILD_DIR=$DIR/pos-system-api-devkit-package
ROOT_DIR="$(cd "$DIR/.." && pwd)"

echo "======================================================================================="
echo "DIR=$DIR"
echo "BUILD_DIR=$BUILD_DIR"
echo "ROOT_DIR=$ROOT_DIR"
echo "======================================================================================="

function header {
    echo "======================================================================================="
    echo $1
    echo "---------------------------------------------------------------------------------------"
}

# install depednencies
header "Installing dependencies..."
sudo apt update
sudo apt install -y pandoc texlive-latex-base texlive-latex-recommended librsvg2-bin zip

# setup directories
header "Setting up build directory..."
rm -Rf $BUILD_DIR
mkdir -p $BUILD_DIR


# clean the projects (clean all **/bin and **/obj folders)
header "Cleaning projects..."
find $ROOT_DIR -type d \( -name bin -o -name obj \) -print -exec rm -rf {} +

set -e

# create PDFs out of the markdown files
header "Creating PDFs from markdown files..."
for mdfile in $(find $ROOT_DIR -iname "*.md"); do
    pdf_file="${mdfile%.*}.pdf"
    pandoc "$mdfile" --resource-path="$(dirname "$mdfile")" -o "$pdf_file"
    echo "Created PDF: $pdf_file"
done


###################################################################################################################
### POS System API docs - temporary solution until officially released

# get API docs (TEMPORARY SOLUTION)
#header "Getting POS System API docs..."
#echo "TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO "

# generate API docs out of swagger file
#header "Generating POS System API docs from swagger file..."
#echo "TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO TODO "

###################################################################################################################


# copy to build directory
header "Copying files to build directory..."
echo "Copying howtos..."
cp -r $ROOT_DIR/HOWTO* $BUILD_DIR
echo "Copy lib..."
cp -r $ROOT_DIR/libPosSystemAPI $BUILD_DIR
echo "Copy scripts..."
cp -r $ROOT_DIR/mitmproxy.bat $BUILD_DIR/
echo "Copy docs..."
cp -r $ROOT_DIR/*.pdf $BUILD_DIR/
echo "Copy POS System API and Compliance docs..."
cp -r "$DIR"/artifacts/* "$BUILD_DIR"/

# clear markdown files from build directory
header "Removing markdown files from build directory..."
find $BUILD_DIR -iname "*.md" -print -exec rm {} +

header "Build completed."
echo "Build output located at: $BUILD_DIR/"
