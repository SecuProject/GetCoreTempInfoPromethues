# GetCoreTempInfoPromethues

It tool will retrieve the temp and the load of the CPU from Core Temp and parse them for Promethues. 
GetCoreTempInfoPromethues generate a file `C:\Program Files\windows_exporter\textfile_inputs\CoreTemp.prom` and update it every 15 seconds. 

## Core Temp 

The software `Core Temp` can be found here:
https://www.alcpu.com/CoreTemp/

## Recommendation

It is recommended to start the process `Core Temp.exe` and  `GetCoreTempInfoPromethues.exe` when the user log in.

This can be do with the following command:

```
reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run" /v GetCoreTempInfoPromethues /t REG_SZ /d "PATH_OF_GetCoreTempInfoPromethues"
```

## Example 

This a example of the output data:

```
# HELP windows_coretemp_cpu_temp Metric coming from Core Temp
# TYPE windows_coretemp_cpu_temp counter
windows_coretemp_cpu_temp{core="0,0"} 61.375
windows_coretemp_cpu_temp{core="0,1"} 61.375
windows_coretemp_cpu_temp{core="0,2"} 61.375
windows_coretemp_cpu_temp{core="0,3"} 61.375
# HELP windows_coretemp_cpu_load Metric coming from Core Temp
# TYPE windows_coretemp_cpu_load counter
windows_coretemp_cpu_load{core="0,0"} 1
windows_coretemp_cpu_load{core="0,1"} 7
windows_coretemp_cpu_load{core="0,2"} 4
windows_coretemp_cpu_load{core="0,3"} 1
```

## Error

GetCoreTempInfoPromethues.exe will return a error message in the case Core Temp is not running.
