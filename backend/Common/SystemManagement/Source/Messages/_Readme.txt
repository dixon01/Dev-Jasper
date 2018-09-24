IMPORTANT: DO NOT CHANGE NAMESPACES IN THIS PROJECT
===================================================
This project contains classes with wrong namespaces, this is intentional to
prevent issues with backwards compatibility regarding BEC serialization when
using pre-2.4 System Manager dependent components with System Manager 2.4 and
vice-versa. This can especially happen when updating components
(e.g. Update 2.4 with System Manager 2.2).

Also for this reason, all .cs files end in .g.cs to prevent StyleCop from
analyzing the files (and figuring out that they are in the wrong namespace).