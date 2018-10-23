## About SBRFPinpad ##
SBRFPinpad is a SBRFSRV.Server (sbrf.dll) C# wrapper

## Features ##
- Purchase/Refund
- Pinpad ready check
- Get pinpad SN
- X&Z Reports
- Transaction cancellation

## Usage ##
- Copy locally:
	SB_KERNEL.DLL
	GATE.DLL
	SBRF.DLL
	LOADPARM.EXE
	RRDK.R
	R003.R
	PARAMS.TLV
- Register SBRF.dll (regsvr32 SBRF.dll)
- Connect Pinpad to USB or COM port
- Set COM port in pinpad.ini

using (var pp = new SBRFPinpad.SBRFPinpad()) 
{
	pp.SetAmount(100);
    var code = pp.Purchase();
    if (code != 0)
	{
		throw new Exception();
    }
}

## License ##
-------
[GNU General Public License v2](http://opensource.org/licenses/GPL-2.0)