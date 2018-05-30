# 18w-paintthetown
Paint the town is a mobile Virtual Reality game in which users can claim landmarks and buildings for their team around their real-time
location by painting them with their team's color. Our vision is for users to be able to walk down their city street, hold up their phone
to nearby buildings, and compete with other local users for team ownership by covering them with virtual paint!

# Backend
Our server runs through Heroku, where our scripts request and send information to coordinating Heroku URLs. We use Wrld3d's API for 3d 
geolocation-enabled maps to population buildings into wide spans of our game's map. Our [backend solution](https://github.com/dartmouth-cs98/18w-paintthetown-backend) 
handles login, authentification, all major user data (challenges, stats, states), team data, building data,
paint data, and more. 

# Frontend
We are building our entire frontend in Unity3d, including all UI elements and features like GPS location. The frontend environment 
consists of multiple Unity 'scenes' - each of which serves as a different screen within the user flow. Starting with either the `SignUp` 
or `Login` Scene, the user enters the game. If the user is signing up, it's the first time they've entered the game, so they are directed 
to the `TeamAssignment` Scene. 

From there, users enter the main game scene where they can see a bird's eye view of their surroundings. In the top right corner of this
scene, there is a spray paint can, which functions as a button to toggle back and forth between bird's eye view and street view.

For special "Points of Interests", the user will see a large spray paint icon above the special building. When they click there, they will
be jumped to a `POI` Scene, where they can paint in an advanced environment that allows for 3D orbit around the building.

To navigate between the `Challenge` Scene and the main game scene, to logout, or to mute the sounds, use the menu button in the top right 
corner of the screen. This button will slide out a menu from the left-hand side of the screen.

# Setup
Build and run with Unity 5 (gpu supporting DirectX 9 or higher required) on either iOS or Android platforms.
  ## Build to iOS:
  - Make sure your AppleID is linked to your XCode. `XCode > Preferences > Accounts`
  - In the `Build Settings` within Unity, switch to the iOS platform and make sure all of the target scenes are added to the list of 
  active scenes. 
  - In the `Player Settings` within Unity, set the `BundleIdentifier` to an appropriate name (i.e. `com.unity3d.PaintTheTown`). 
  - Back in the `Build Settings`, click `Build`. 
  - Once Unity successfully completes the build, open the `.xcodeproj` file, which will open in XCode.
  - In XCode, you may need to select your `Developer Team`. `Unity-iPhone > General > Identity > (select your account from the drop-down 
  menu)`


# Setting up the repository
  ## .gitignore
  Many Unity files are only meant to remain in the local environment, as pushing them to the github repo will cause a wide range of 
  issues. These include build, project, and user files as well as MacOS and Windows specific files. Since our team uses two different 
  platforms, it's important to keep files specific to one quarantined.
  
  Our .gitignore file has all the file extensions that we do not need. We sourced from **[Unity's recommended .gitignore]
  (https://github.com/github/gitignore/blob/master/Unity.gitignore)**, though it lacked any OS-specific extensions. For that I sourced
  from this **[MacOS template](https://github.com/github/gitignore/blob/master/Global/macOS.gitignore)** and **[Windows template]
  (https://github.com/github/gitignore/blob/master/Global/Windows.gitignore)**
  
  ---
  ## Git-LFS
  Git Large File Storage tracks large files in Git using the .gitattributes file. This allows us to keep these files out of our actual 
  repository. In order to save space, Git LFS avoids downloading the files it tracks, leaving them on the remote for retrieval as needed.
  This is important as Unity handles massive amounts of large files (Assets anyone?) which can hinder the pushing and pulling process. We 
  used a template file from ***[Rick Riley at thoughtbot.com](https://robots.thoughtbot.com/how-to-git-with-unity)*** 
  
  ---
  ## Git Hooks
  I copied this resource as I could not say it better myself. All credit goes to Forrest Smith at 
  https://blog.forrestthewoods.com/managing-meta-files-in-unity-713166ee3d30
  
  In addition to using a .gitignore file to avoid Git collisions, we use Git Hooks that ensure Assets and their respective meta files are
  playing nicely. This ensures that there is no catastrophic failure of the Unity repository.
  
  ### How Asset and Meta files interact
  Every asset file stored in Unity has a meta file associated with it. Because such information cannot be contained in the asset 
  filetypes, it's crucial that an asset and it's meta file are consistent with each other. That means that if an asset is deleted, so
  should the meta file and vice versa. 
  A guid is a globally unique identifier. Every asset has a meta file and every meta file has a unique guid. The guid is randomly 
  generated when the meta file is created. Most pain and suffering related to meta files has to do with this guid.
  In addition, ***absolutely do not manually change guids.**
  
  why?
  
  Changing a guid breaks all references and generates behavior missing errors. You should never ever change a guid. The good news is you 
  never change guids intentionally. The bad news is you can ***indirectly cause it to happen.*** Duplicating a guid is also catastrophic. 
  If Unity encounters a duplicate guid then it will replace the one of the two with a newly generated value. That means Unity changed a 
  guid. Which is catastrophic.
   
   Given that changing a guid is catastrophic and by extension duplicating guids is catastrophic there are a handful of user operations
   which can cause either catastrophe to happen.

### 1. Submitted new asset without meta file
Easily the most common mistake. Especially when using source control. You created a script or texture but forgot to the submit the 
matching meta file.

This mistake cascades into catastrophe. If you forget to submit a meta file and I sync then I will generate a new meta file. However my 
file will have a different guid than your file. If you also checked in objects with references to your guid then on my machine those 
references will become missing behavior errors.

When this case happens the person who originally created the file must submit their meta file. If I submit my meta file then when you sync
you’ll get a new meta file with a new guid which is changing a guid which is catastrophic.

### 2. Unpaired asset/meta file
Assets and meta files should always be created and deleted as a pair. When using source control you also need to submit add operations or
delete operations as a pair.

Here are three different mistakes which can be made. These mistakes are uncommon so I’m lumping them together.

Submitted meta file without submitting asset.
Deleted asset without deleting meta file.
Deleted meta file without deleting asset.
These are all straight forward and easy to fix. They are more disruptive and confusing than catastrophic.

### 3. Manually copied asset + meta file to new location
If you copy an asset and it’s meta file in Windows Explorer/OS X Finder then you have a duplicated guid. Which is catastrophic.

When moving assets always do it through the Unity editor. Unity correctly handles moving assets and their meta files.

### 4. Moved asset but left old file in source control
When you move an asset through the editor there are two source control operations. You are deleting old files and adding new files. You
must submit both operation sets.

If you submit only the add operation but not the delete operation then your machine will be fine. When I sync my machine I will have both
the old files and the new files which means a duplicate guid. Which is catastrophic.

### 5. Empty folders generating meta files
Unity generates meta files for folders. Perforce ignores folders entirely. If all the files are deleted in a folder and you sync Perforce 
it will delete the files but leave the empty folder. If the folder meta file was deleted then Unity will see the empty folder and generate
a new meta file. Urgh!

It’s somewhat harmless. However if you reconcile with Perforce it’s super important to have a “clean” reconcile. If there is cruft then 
you’re more likely to submit an error that does matter.

I use a tool to remove empty directories. I run this tool periodically after syncing latest.

### Preventative Measures
We’ve established that there are certain catastrophic failures and they can happen in a variety of ways. The good news is that knowing is 
half the battle! The other half is writing tools to detect errors before they are submitted to source control.
  
  The link below contains all the files necessary to accomplish this. There are 4 files total, 3 of which are the actual hooks. They're 
  called 'pre-commit', 'post-checkout', and 'post-merge'. The final file is called 'install-hooks.sh', which ***needs*** to be run before
  the hooks are installed. Once finished you can rest easy and not have to worry about such collisions, though it's recommended that you
  also use the knowledge on how Assets and Meta files interact to inform your future actions.
  
  
### [Link for the GitHub repo for the Unity Hooks](https://github.com/Shoopalapa/unity-git-hooks)
  
  

