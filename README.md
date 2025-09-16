1. Ensure character is Humanoid and add animator

2. Enable IK pass on animation layer

3. Add animations such as walking/idle and enable Foot IK

4. Attach "EnableFootIKInAnimation.cs" in animation behavior

5. Attach "NewFootIk.cs" to the object with the animator and adjust settings
   - Assign ground layer
   - Assign stair layer (optional)
   - (Optional) For smooth stair movements, create two identical stairs. Make one convex and the other non-convex. Assign a "Stair" layer to the non-convex collider.
     -  Character Controller components usually teleports the character up stairs. This creates jittery movement.
     -  By utlizing both convex and non-convex colliders, you will maintain smooth movements from flat slopes while still having realistic foot placements.

   
