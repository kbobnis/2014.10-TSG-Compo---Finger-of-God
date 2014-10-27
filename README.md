# README #

Unity. Just open and play.

How to make maps.

# Use tiled map editor #
1. Create map 5x7 32x32px
2. Import tileset from Images
3. Create Map using these tiles
4. Map -> Map properties. add custom property (
	Interventions, int this is the success query) 
	BeforeText to write a text before this map add custom property
	BeforeImage and number of corresponding building from the tileset starting from 1
	TipText a text with a tip if user will fail level
5. save ast json (no need to save tmx) as mapX.json. This map will be loaded as mapX when doing missions.


# Google analytics:
https://github.com/googleanalytics/google-analytics-plugin-for-unity