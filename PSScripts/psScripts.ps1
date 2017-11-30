Connect-Graph -Endpoint "<put your endpoint>" -Authkey "<put your authkey>" -DBName "graphdb01" -CollectionName "app"
Add-Vertex -Vertex "test"


################################
$packagesConfigPath = "C:\Repo\Github\My\psCosmosGraphTest\packages.config"
$packagesConfig = [xml](gc $packagesConfigPath)

$packagesList = $packagesConfig.packages.package | select id, @{l = "name"; e = {"$($_.id)-$($_.version)"}}

Add-Vertex -Vertex ($packagesList.name) -Verbose

#$packagesList.name | % {
#    Add-Vertex -Vertex $_ -Verbose | Out-Null
#}

#$packagesList | % {
#    $current = $_
#    $pName = $current.name
    
#    $pName
    
#    $p = Find-Package $current.id -AllVersions -ProviderName NuGet | select -First 1
#    $deps = $p.Dependencies | % {($_ -split "`:")[1]}

#    $deps | % {
#        Add-Edge -From ($pName -replace "/","-") -To ($_ -replace "/","-") -Verbose
#    }

#}

$packagesList | % {
    $current = $_
    $pName = $current.name
    
    $pName
    
    $p = Find-Package $current.id -AllVersions -ProviderName NuGet | select -First 1
    $deps = $p.Dependencies | % {($_ -split "`:")[1]}

	if ($deps) {
		$edges = 
		$deps | % {
			$from = $pName -replace "/","-";
			$to = $_ -replace "/","-"
			[pscustomobject]@{from = $from; to = $to }
		}
	
			Add-Edge -edges @($edges) -Verbose
	}
}



