A simulation for an automated vacuum cleaner in Unity. THis project implements the agent-based model to create a simulation of a vacuum that will go clean up the environment.

Agent-modal - simple reflex 


Percept (Info that can be sensed), Actions (Things that can be done) 
    -> Sensor (figures out what info can be sensed ) -> Percepts
    -> Mind (Use the info in the percepts to  determine action) -> Actions
    -> Actuator(Acts on the actions the mind decided) 

Performance measure
- How much dirt is sucked up
- How many tiles are clean 
- How much energy is used

****************************************************************************************************
What you will see when you run the simulation...
First, notice how each tile gathers dirt at a different rate. This will influence where the vacuum goes.
The vacuum is set to go after the dirtiest tile in its sight. 
Wait for the point where the robot is satisfied with the job it has done in the first two tiles. It will then move around until it notices how dirty tile 4 is. 
When it gets there it will notice that tile 5 is even dirtier so it will go to clean it first. Then when it goes to handle tile 4 it will notice that it cannot suck any dirt out of it
THat's because this tile is a carpet and has a higher resistance. So, the bot will go into turbo mode - which will spam warning messages in the console telling you that is turbo mode. To make this easier to see you may want to toggle the normal debug messages in the unity editor and only see the warnings. 
Then it will move on to the next dirties tile. Notice that the bot goes into turbo mode only in tile 4 but quickly switches off to normal mode when it moves off it. THat's because turbo mode has a lower efficiency rate.
So, it prefers to be in normal mode whenever it can. 
Currently, I just print out the efficiency rate of the bot. If you are looking at how the efficiency rate changes you might want to turn off the warning messages from the unity editor to help you keep better track of it. 