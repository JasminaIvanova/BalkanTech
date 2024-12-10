# BalkanTech
Softuni project ASP.NET CORE

BalkanTech is a project for managing tasks for hotel Balkan. It has the following functionalities: 

It has 3 roles: Admin, Manager, Technician. Every one of them have different responsibilities
1. Manage Rooms
    -Add Rooms - Only admin
    -View Rooms - everyone
    -Mark as Occupied/Available - everyone
    -Assign tasks for specific room - only Manager
    -Search bar for easy search of a room
    -pages for rooms
2. Manage Tasks
    -Add task - Only Manager
    -Edit/Delete task - only Manager
    -View tasks - everyone
    -Filter tasks based on category
    -pages for tasks based on completed/to be completed
    -Details of given task 
        -everyone can leave a note under the task. Once written note, It can't be deleted/edited
3. Manage room categories/task categories
    - Add/Delete/View all categories - Only admin
4. Reports
    -If a user is Technician, he can see its current assigned tasks and only he can manage their status to "In process/Completed".
    Given task can be completed only from Manager, before it its status is "Pending Manager Approval"
    -If user is Manager, he can see the assigned tasks based on a specific technician and confirm completion of the tasks
5. Admin Panel
    -Only admin can manage the roles of the users, if they are manager/technician.
    -Only admin can create users, no one can register on their own
    -Only admin can manage categories/rooms (room can not be deleted, only added and marked as available/not available)
    




