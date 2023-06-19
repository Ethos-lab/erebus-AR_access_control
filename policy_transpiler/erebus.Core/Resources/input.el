function Raycast()
{
	let curLoc = GetCurrentLocation();
	let trustedLoc = GetTrustedLocation("Home");
	let curFace = GetCurrentFaceId();
	let trustedFaces = GetTrustedFaceId("Batman");
	if ( curLoc.within(trustedLoc) )
	{
		if ( curFace.matches(trustedFaces) )
		{
			Allow;
		}
	}
}
function GetPlane()
{
	let curLoc = GetCurrentLocation();
	let trustedLoc = GetTrustedLocation("Home");
	let curFace = GetCurrentFaceId();
	let trustedFaces = GetTrustedFaceId("Batman");
	if ( curLoc.within(trustedLoc) )
	{
		if ( curFace.matches(trustedFaces) )
		{
			Allow;
		}
	}
}
function ARPlaneTrackables()
{
	let curLoc = GetCurrentLocation();
	let trustedLoc = GetTrustedLocation("Home");
	let curFace = GetCurrentFaceId();
	let trustedFaces = GetTrustedFaceId("Batman");
	if ( curLoc.within(trustedLoc) )
	{
		if ( curFace.matches(trustedFaces) )
		{
			Allow;
		}
	}
}
function RegisterEventOnPlanesChange()
{
	let curLoc = GetCurrentLocation();
	let trustedLoc = GetTrustedLocation("Home");
	let curFace = GetCurrentFaceId();
	let trustedFaces = GetTrustedFaceId("Batman");
	if ( curLoc.within(trustedLoc) )
	{
		if ( curFace.matches(trustedFaces) )
		{
			Allow;
		}
	}
}
function UnRegisterEventOnPlanesChange()
{
	let curLoc = GetCurrentLocation();
	let trustedLoc = GetTrustedLocation("Home");
	let curFace = GetCurrentFaceId();
	let trustedFaces = GetTrustedFaceId("Batman");
	if ( curLoc.within(trustedLoc) )
	{
		if ( curFace.matches(trustedFaces) )
		{
			Allow;
		}
	}
}