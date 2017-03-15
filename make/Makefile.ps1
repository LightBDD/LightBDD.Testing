Define-Step -Name 'Update version' -Target 'all,build' -Body {
    function Replace-InFile([string]$file, [string]$version, [string[]]$matchings)
    {
        Write-ShortStatus "Updating $file with $version..."
        $content = Get-Content $file -Encoding UTF8
        foreach($match in $matchings)
        {
            $from = [Regex]::Escape($match) -replace '%','[0-9]+(\.[0-9]+)*'
            $to = $match -replace '%',$version

            $content = $content -replace $from,$to
        }
        Set-Content $file -Value $content -Encoding UTF8
    }

    $version = (Get-Content 'make\current_version').Trim()
    Write-ShortStatus "Updating version to $version..."

    gci -Filter 'project.json' -Recurse | %{ Replace-InFile $_.fullname $version '"version": "%", //build_ver','"version": "%-pre", //build_ver' }
}

Define-Step -Name 'Build' -Target 'all,build' -Body {
    call dotnet restore
    call "msbuild.exe" LightBDD.Testing.sln /t:"Build" /p:Configuration=Release /m /verbosity:m /nologo /p:TreatWarningsAsErrors=true /nr:false
}

Define-Step -Name 'Tests' -Target 'all,test' -Body {
    . (require 'psmake.mod.testing')

    $tests = Define-DotnetTests -TestProject "*.Tests"

    $tests | Run-Tests -EraseReportDirectory -Cover -CodeFilter '+[LightBDD.Testing*]* -[*Tests*]*' -TestFilter '*Tests.dll' `
         | Generate-CoverageSummary | Check-AcceptableCoverage -AcceptableCoverage 80
}

Define-Step -Name 'Packaging' -Target 'all,pack' -Body {
    Remove-Item 'output' -Force -Recurse -ErrorAction SilentlyContinue | Out-Null
    mkdir 'output' | Out-Null

    gci -Path "src" -Filter 'project.json' -Recurse `
        | %{ call dotnet pack $_.fullname --output 'output' --no-build --configuration Release}
}