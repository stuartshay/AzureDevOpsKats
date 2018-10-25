while($true)
{ 
    $i++
    
    $output = docker-compose --file docker-compose-staging.yml down | Out-String
    $output2 = docker-compose --file docker-compose-staging.yml up | Out-String
    
    Write-Host "Action has run $i times" 
 }
