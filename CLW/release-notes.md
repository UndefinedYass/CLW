

planned:
- news section: implement lazy loading with "show more" button to reduce latency, or pages system
- major: news are saved in appdata, also ref file is saved there under a folder named after the preset's guid
- watcher properties window 
- watcher quick context menu for most important settings in properties window
- warn before overridng file on download command
- about window; and appInfo service that holds thngs like app version
- add functionality for the global enable/disable watchers; without losing the watcher-wise enabled state information, same for notifications, implement these commands in the tray context UI
- enhance error system
- invent user friendly exceptions handeling system for the watching and parsing functions
- notification window show same avatar/icon as in the watcherView
- notification window should be somehow closable without showNews or markeSeen actions: close button appears when the window is hovered
- OPT: remove or minus button next to every preset declaration
- ADD: notification window: add shortcut command for: - disabling the watcher's notification,s or disabling the notifications globally, like chrome's notifictions
- Settings: notifications arrangment style: stack or overlay, requires building the infrastructure for arranging notifWindows on the screen
- Settings: open mainWindow at strartup?
- Arg: add new arg specifying whether to show window or not, 
- Arg: handle files args performing loadWatcher actions foreach file
- implement the functionality for DownloadAll 
- implement the functionality for MarkSeen, using a temporary approach that engages a pre-saved list of read news's ID's
- DEV: implement after-building macros that makes the innoSutep exe
- Opt: Notifications desapear automatically after a while, unless the user is inactive then they stay around
- add: notifications disappear immediately when user selectes thier target watcher
- BUG: app chrashed when not even focused, possiblly while doing a check
- BUG: trying to download while another downloading is going on can show error, saying curl exited with code 0
- BUG: download, (and fetching) using Native takes quite a while, maybe the proxy property needs to be set before every request
- Maj: the CPE graph should not stay around for the rest of the watcher's lifetime, 
 as it takes alot of memory, instead it should be used only to generate an
  "exctraction function" for every required TargetProperty,
 these functions take the raw Item string and return whatever content the TargetProperty aims to find
 also an "Items exctractor" function should be generated, that takes the raw page and returns
 a list of raw items
 these two elements will work together to do the parsng on every check,
 while the crrent cpe parsing approach works well for small presets it gets inefecent in heavy loads
 and intermediate solution is to use an htmlParser-base Item CPE, instead of using regex
 MAJ: use system.linq.expressions to go about the mentioned major modification
bug: startng or stopping all watchers commands may cause fatalerrors on some
fix: watcher body section should reftect the watcher state in details:
 thses ui elements should show up at the right time with the right contetnt:
 error card, no news card, uninitalized notice, fatal error card, news list
 there should be a well tuned logic behide each element's visiblity
 e.g error card should be hidden if there are news that has been shown, regrdless of the failing state
 these of course requre some UX thinkng, but there is something more straiforward to start with:
 the basic watcherViewModel properties that will be the source of data some of theme are there
 some of them are not fully implemented, some of them require a Model counerpart :
 -IsFailing -IsRunningSuccessfully, HasFatalError, FailingException, FatalException,
 HasNews, NewsCount, LastCheckDate, IsInitialized, IsWatching, StatusMessage, 
 HasDoneSuccessfulChecks

 -UI: both error card and fatalError card should have an exception stack expander that
 allow the user to browse throu the inner excepions, and see their information
 - switch to using statc resources instead of dynamic as they're more efficient
 - performance: declare resources at app.xaml level to avoid creating them multiple times: i,e the news item view might contain recources that gets created as many times as the number of news, 
 migrating those recources to the app level would make noticeable performance improvment
UI: multiple data templets for the new tem view each is optimized for each case: with / without content
with/without downloads
-ADD: news filtering feature: custom rules at the item level in the preset that enabels user to
control and restrict the content theyre willing to get notified about;
this might be usefull when, say, an fb source puts alot of content out there but we're only interested of the posts that conclude certain keywords
this may requre a new CLWP version with a few more elements yet keep backwards compatibility with 2.0
-UI: add some coloring fedback on the button that opens the downloads$ed file to inform the user
 whether the file has been opened (in the current session) 





////////////////////////////////////////////////////////////
v0.4.0 (from  05-10-2021 7:05AM to -10-2021 : ) []

done:
06-10-2021 day accoumplishments:
-droped ram usage drastcally by reusing the CPE Item element, making use of refresh function, instead of spawnng new elements  tree foreach piece of news
this is still a temporary solution, whle planning on the Expressions approach

implemented a temporarty sorting solution: Special property "SortBy"
solved string encoding problem, unicode  characters now show up correctely
replaced the old "interface" icon with whiteOnBlack node-eye icon
oprimized parsing time, reduceing code, a 1200 watcher now takes 0.6sec in averge to parseTheList

- settings window: added a bunch of settings and their bnding and functionality: mac concurrency, open wndow at startup,
- bug: stoping a wathcher that is failing causes some state of deadlock that prevents all watchers from working again
    solved: turned out the error itself costs sem place, as it never release itback: solved by releasing the sem anywhere when leaving the check fucntion body using a finally cleanup approach 
- added about tab in settings window
- added watcher properties window, ui and view model only



////////////////////////////////////////////////////////////
v0.4.0 (from 27-9-2021 8:45PM to 05-10-2021 7:05AM ) [released]

done:
- added the ifrastructure for programmatically changing sysTray icon : implemented at the Utils class. [28sept 3:43AM]
- CPML: added "color" attrib that specifies the watcher's bg color [5:06AM]
- implemented utils.UpdateSysTrayToolTip text, now tool tip shows some unread news count info
- show document type (e,g PDF) in the NewItem View  [7:38AM]
- UI: experimentaal floating add button at the bottom of the watchers list
- alloing and handeling file drag-drop behaviour on the watcher's list to loead watchers from presets
- allow resizng withgrip: basic temporary solution with window transarency allowed 11:39AM and lets call this day
- made the app only run sngle instance, adapted mutext and waitHanfle approach [28-sept 8:27PM]
- ShowLastNotification command in SsTray context is now funtional, 
- UI: solved the freezing UI when doing parsing and web stuff,: migrate heavy operation to separate threads, making UTils methods such as spawning notifications thead-safe i,e callable from other threads 
- ListWatcher class now extends BackgroundWorks providing robust multithreading, cancelation support, concurrency control through semaphore, and few more UI-friendly events for the consumer
- add a way to open file when downloaded, 
- UI: add feedback on checking : spinner

fixes:
- BUG: stoping watcher while it's waiting for semaphore causes fatal exception : handeled operationCanceledException


////////////////////////////////////////////////////////////
v0.3.0 (from 21-9-2021 4:49 to 27-9-2021 ) [unreleased]
- download button: remember the output directory for downloads and only show folder picker
if control + click gesture was performed [done 22-sept 2:56AM]
- ui: NItemUC: hidden mark as read button untill mouse is over 
- sort news based on date, and show the most recent Item on the notification
- avatar colors, randomly generated
Major:
- system tray icon, with context menu: partially functional: app dont qhut down when closing mainWindow 
- switched to MVVM: deprecated Session singl tone, an serval code behind and xaml files, created the folders structur for mvvm
- started switching clw parser specifications to v2.0: using AngleSharp as core html parser
- settings window: functional presets declarations data grid


////////////////////////////////////////////////////////////
v0.2.0 (21-9-2021 4:49) [unreleased]

-Switched to MaterialDesign UI 
-enhanced UI: expandable Watchers section, experimental Shorthand controls for noifs and sound

-added the SnackBar messages system
-removed annoying messageBox on successfull downloading, replaced with snackBar message with "Open" action
-enhanced NotificationWindow UI: removed the download button, switched to MD, ..













//////////////////////////////////////////////////////////////
v0.1.0-fbhd-beta (?-?-2021 ?:?) [first pre-release] 


Equivalent to fbhd 0.6.0, most of the code was copied and slightely modified to suit the new UI 




