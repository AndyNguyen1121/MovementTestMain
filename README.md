1. Ensure character is Humanoid and add animator

2. Enable IK pass on animation layer. Add a bool "FootIK" to parameters

3. Add animations such as walking/idle 

4. Attach "EnableFootIKInAnimation.cs" in the animation behavior to the animations that you want to apply FootIK to

5. Attach "NewFootIk.cs" to the object with the animator and adjust settings
   - Assign ground layer
   - Assign stair layer (optional)
   - (Optional) For smooth stair movements, create two identical stairs. Make one convex and the other non-convex. Assign a "Stair" layer to the non-convex collider.
     -  Character Controller components usually teleports the character up stairs. This creates jittery movement.
     -  By utlizing both convex and non-convex colliders, you will maintain smooth movements from flat slopes while still having realistic foot placements.

   
