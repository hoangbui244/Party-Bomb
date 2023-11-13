# Changelog

## Version 3.2.0 (The XR Update)

Added support for VR interactions with integration for XR Interaction Toolkit and Oculus Interaction SDK.

### NEW FEATURES:
- New 'Flexalon XR Input Provider' allows Flexalon Interactables to be used with interactables from XR Interaction Toolkit.
- New 'Flexalon Oculus Input Provider' allows Flexalon Interactables to be used with interactables from Oculus Interaction SDK.
- New 'Insert Radius' property for Flexalon Interactable allows you to specify how close a dragged object needs to be
  to a layout before it is inserted.

### FIXES AND CHANGES:
- Flexalon Drag Target no longer creates a collider on itself. This prevents it from interfering with physics interactions.
  Instead, it uses a custom overlap detection function with the new Insert Radius property of Flexalon Interactable.
- Flexalon Interactable's generated placeholder no longer has a box collider for the same reason.
- If a Flexalon Interactable is dragged into two drag targets at once, it will now select the one with the nearest child.
- Flexalon Interactable local space restrictions no longer change to world space when leaving a drag target.
  This was causing objects to jump when leaving a drag target in some scenarios. Instead, the interactable continues to use the
  last drag target's local space until a new drag target is detected.
- Fixed a bug in which layouts were not always recomputed correctly when adding children from another layout.
- Fixed a bug in which layouts were not always recomputed correctly when a child without a Flexalon Component was deleted.
- Fixed a bug in which a Flexalon Object's offset would sometimes change unexpectedly.
- The Flexalon Result hidden component will no longer appear when a prefab is selected in the asset browser.
- The Game Object > Flexalon context menu will now add new layouts under the right-clicked gameObject.

## Version 3.1.0

Added new start screen and trial version.

## Version 3.0.0

Another major update with 3D grids, infinite curves, and more!

### NEW FEATURES:
  - Grid Layout:
    - Added a new 'Layers' property allows you to create 3D grids.
    - Added a new 'FlexalonGridCell' component allows you to specify which cell an object should be placed.
    - Added a new 'Cell Size' property allows you to specify a fixed cell size instead of dividing the grid size.
    - Added a new helper functions to retrive children in a grid column/row/layer.
  - Curve Layout: Added new BeforeStart and AfterEnd properties that allow you to create infinite curves:
    - Ping Pong: Extend the curve by continuing in the opposite direction.
    - Extend Line: Extend the curve in a straight line based on the tangent at the start/end of the curve.
    - Repeat: Extend the curve by repeating the curve.
    - Repeat Mirror: Extend the curve by mirroring the curve and repeating it.
 - Interactable:
   - Added a new 'Collider' property to specify a different collider object for click/drag.
   - Added a new 'Margin' property to FlexalonDragTarget which increases the size of the target.
 - FlexalonRigidBodyAnimator now supports RigidBody2D.
 - Scripting API docs are now available at flexalon.com/docs/api

### FIXES AND CHANGES:
 - Interactable Fixes:
    - Local space restrictions and offsets now update which local space is used when dragging between layouts. For example, if you drag a book from horizontal stack of books to a vertical stack of books, the book will rotate to match the hovered stack.
    - Dragging an object no longer centers the object on the mouse.
    - Placeholder size calculation is more accurate.
 - Curve Layout Fixes:
    - Forward, Backward, InWithRoll, and OutWithRoll options now rotate objects correctly when the curve turns upside down.
    - Serialized FlexalonCurveLayout.CurveLength so it is always available.
 - Performance Improvements:
    - Flexalon debug logs are completely compiled out of the build.
    - Improved efficiency of layout editors.
    - Added preprocessor checks to automatically disable features that depend on optional Unity packages.
 - General Fixes:
    - Flexalon layouts no longer apply when disabled.
    - Fixed a bug where adding and removing FlexalonObject component causes weird scaling.
    - Fixed a bug where removing a child from a layout didn't always update the layout.
    - Fixed a bug where editing the transform of a FlexalonConstraint in the editor would update the offset and rotation incorrectly.
    - Fixed the offset for rotated objects with a non-zero component bounds.

## Version 2.0.0

This BIG update to Flexalon includes new layouts, interactions, bug fixes, and more!

### NEW FEATURES
 - Align Layout: Align all children to the parent on the specified axes
 - Shape Layout: Position children in a shape formation with a specified number of sides. Great for crowds and unit formations.
 - Flexalon Interactable: Add click and drag interactions which let users add, remove, and swap objects in layouts.
 - Random Modifier: Add to any layout to randomly modify the positions and rotations of the children.
 - Circle Layout: New 'Radius Type' option can modify the radius for each object or for each iteration around the circle.
 - Curve Layout: New Tangent Mode options can automatically generate smooth or corner tangents. New spacing option 'Evenly Connected' can evenly space curves that have connected start and end. New 'In With Roll' and 'Out With Roll' rotation options.

### BEHAVIOR CHANGES
 - Curve Layout's spacing mode 'Evenly' now places the first object at the start of the curve and the last object at the end of the curve.
 - Curve and Lerp animators now operate in world space, which makes it simpler to transition objects between layouts.
 - TransformUpdater interface now requires a PreUpdate method, which is called before layout starts updating transforms. This can be used to capture the current transform state.
 - A Flexible Layout that does not have Wrap checked will now use the full size of the layout when computing the fill size on both of the non-flex axes. When Wrap is checked the behavior is unchanged: the fill size on the wrap axis depends on the size of each line.
- Improved the behavior of a spiral layout with negative spacing.

### FIXES
 - Flexalon will now update automatically when a new layout component is added.
 - Fixed an issue where Flexalon would sometimes update on recompile.
 - Multi-editing objects previously updated Flexalon once for each object. Now it only happens once.
 - Fixed cases where Flexalon Constraint prevented using the Unity transform control.
 - Fixed the layout size calculation of grid layout with hexagonal cell type.
 - Fixed some cases where the Flexalon Object bounding box visual appears in the wrong place.
 - Fixed NaN errors when a curve layout has two points in the same position.
 - Fixed some instances where Flexalon Result component appears in the inspector (it should always be hidden).
 - Readme asset will only be selected the first time Flexalon is imported.

## Version 1.0.2
  - Spiral 'Use Height' property replaced with flex-like behavior.
  - New 'Spiral Spacing' property adds vertical gaps between spiral objects.
  - Improved ability to modify objects with the standard transform tool.
  - Fixed various Undo / Redo bugs.
  - FlexalonObject's offset, rotation, and scale no longer apply when an object is not in a a layout or constraint.

## Version 1.0.1
  - FlexalonConstraint now supports margins.
  - Add help links to Flexalon website documentation.
  - Hide Documentation from Unity by renaming directory to Documentation~
  - Prevent adding multiple layout components to a GameObject.
  - Prevent setting grid rows or columns to 0.

## Version 1.0.0 - Initial Release!