# Bad-Compression-Algorithms
Explores how #s are stored in various Bases (e.g. Base 10 and Base 2), and using either approximations (lossy compression) or different ways of storing #s (lossless compression). I am defining a compression algorithm as useful if it decreases the amount of bits stored in the average case. The algorithms I did test were not very useful.    Here are some algorithms I tested (or wanted to test, given that this project is permanently unfinished):

1) #toStore = RoundUpOrDown( a^(1/x) ). Retrieve # by raising it to the xth power. [lossy]
2) #toStore = a^x + offset [lossless]
3) #toStore = a^x + b^y + offset [lossless]
4) #toStore = a^x + b^y + c^z + offset [lossless]
5) #toStore = a^x + a^y + a^z + offset [lossless]

If you know about infinite series, these algorithms surely look familiar to you.

Hopefully you can find something useful that I can't! Good luck!
