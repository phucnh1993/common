#!/bin/bash
rm -rf ./PhucNH.Commons.Extensions.Tests/TestResults
dotnet test ./PhucNH.Commons.Extensions.Tests --collect:"XPlat Code Coverage"
cd 'PhucNH.Commons.Extensions.Tests/TestResults'
guid=`ls -l | awk '{print $9}'`
guid_length=${#guid}
guid=${guid:0:guid_length-1}
cd $guid
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:HtmlSummary
cd coveragereport
start "" "summary.html"
read -p "Press [ENTER] to finish."