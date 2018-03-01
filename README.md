# 18w-paintthetown
Paint the town is an Augmented Reality game based around claiming landmarks and buildings for your team by painting them with your colors.

# Backend
We use Wrld3d's API for 3d geolocation-enabled maps and make use of facebook and our own backend solution for user authentification and login. 

# Frontend
We are building our entire frontend in Unity3d, including all UI elements and features like GPS location.

# Setup
Build and run with Unity 5 (gpu supporting DirectX 9 or higher required)
.

# Setting up the repository
  ### Git-LFS
  ---
  ### Git Hooks
  In addition to using a .gitignore file to avoid Git collisions, we use Git Hooks that ensure Assets and their respective meta files are playing nicely. This ensures that there is no catastrophic failure of the Unity repository.
  
  #### How Asset and Meta files interact
  Every asset file stored in Unity has a meta file associated with it. Because such information cannot be contained in the asset filetypes, it's crucial that an asset and it's meta file are consistent with each other. That means that if an asset is deleted, so should the meta file and vice versa. 
  A guid is a globally unique identifier. Every asset has a meta file and every meta file has a unique guid. The guid is randomly generated when the meta file is created. Most pain and suffering related to meta files has to do with this guid.
  In addition, ***absolutely do not manually change guids.**
  
  why?
  
  Changing a guid breaks all references and generates behavior missing errors. You should never ever change a guid. The good news is you never change guids intentionally. The bad news is you can ***indirectly cause it to happen.*** Duplicating a guid is also catastrophic. If Unity encounters a duplicate guid then it will replace the one of the two with a newly generated value. That means Unity changed a guid. Which is catastrophic.
   
  
  The link below contains all the files necessary to accomplish this. There are 4 files total, 3 of which are the actual hooks. They're called 'pre-commit', 'post-checkout', and 'post-merge'. The final file is called 'install-hooks.sh', which ***needs*** to be run before the hooks are installed. Once finished you can rest easy and not have to worry about such collisions, though it's recommended that you also use the knowledge on how Assets and Meta files interact to inform your future actions.
  [Link for the GitHub repo for the Unity Hooks](https://github.com/Shoopalapa/unity-git-hooks)
  
  
 
