# Ultima.DLL #
```
 Ultima.Tile mytile = new Ultima.Tile();
 Ultima.TileMatrix tm = new TileMatrix(0, 0, 6144, 4096);
 HuedTile[] htile = tm.GetStaticTiles(777, 1477);
 mytile = tm.GetLandTile(577,1777); // This gets the map tile, so like cave roofs
 int mytileid = (htile[0].ID & 0xFFF); // gets the graphic id of static tiles
```
Clioc usage
```
private static StringList clioclist = new StringList("ENU");
clioclist.Table[cliocID];
```