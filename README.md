# README #

Unity. Just open and play.

How to make maps.

# Use tiled map editor #
1. Create map 5x7 32x32px
2. Import tileset from Images
3. Create Map using these tiles
4. Map -> Map properties. add custom property (Interventions, int this is the success query) to write a text before this map add custom property (BeforeText, string. You can use dynamic values in it {building0.image} will show image of building 0 from tileset. {building0.population} will write number of population from this building. 
5. save ast tmx for future changes. save as json for unity to read in Maps as mapX.json. This map will be loaded as levelX when doing missions.