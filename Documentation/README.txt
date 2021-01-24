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
Features implemented
- Agent can move to selected using three different motors: 
    -RigidBody (via force)
    -character controller
    -Transform Lerp
- Agent will change direction depending on where it is moving
- 

Features to-do
- 

Features future
- 