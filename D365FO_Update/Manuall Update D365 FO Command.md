
AXUpdateInstaller.exe generate -runbookid="Default-runbook" -topologyfile="DefaultTopologyData.xml" -servicemodelfile="DefaultServiceModelData.xml" -runbookfile="Default-runbook.xml"
<br/>

AXUpdateInstaller.exe import -runbookfile="Default-runbook.xml" <br/>

AXUpdateInstaller.exe execute -runbookid="Default-runbook" <br/>

AXUpdateInstaller.exe execute -runbookid="Default-runbook" -rerunstep=[stepID] <br/>
